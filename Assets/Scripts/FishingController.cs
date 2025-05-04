using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// Manages the fishing mechanic including casting, reeling, and fish interactions
/// </summary>
public enum FishingState
{
    Idle,
    Aiming,
    Casting,
    Waiting,
    FishInterested,
    Biting,
    Hooked,
    Reeling,
    Caught,
    Failed
}

public class FishingController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerStatsController playerStats;
    [SerializeField] private GameObject lurePrefab;
    [SerializeField] private Transform castPoint;
    [SerializeField] private LineRenderer fishingLine;
    [SerializeField] private GameObject castingUI;
    [SerializeField] private GameObject bitingIndicator;
    [SerializeField] private GameObject tensionMeter;

    [Header("Casting Settings")]
    [SerializeField] private float maxCastPower = 10f;
    [SerializeField] private float castPowerBuildupRate = 2f;
    [SerializeField] private float aimSensitivity = 1f;
    [SerializeField] private float maxAimAngle = 45f;

    [Header("Fishing Settings")]
    [SerializeField] private float lureSinkSpeed = 2f;
    [SerializeField] private float minCatchDistance = 1.5f;
    [SerializeField] private float biteDuration = 1.5f;
    [SerializeField] private float maxTension = 100f;

    // State tracking
    private FishingState currentState = FishingState.Idle;
    private GameObject activeLure;
    private Lure lureComponent;
    private FishAI hookedFish;
    private Vector2 aimDirection = Vector2.right;
    private float currentCastPower = 0f;
    private float currentTension = 0f;
    private float biteTimer = 0f;

    // Input tracking
    private bool castButtonHeld = false;
    private bool reelButtonHeld = false;

    // Events
    public event Action<FishAI> OnFishCaught;
    public event Action OnFishingFailed;
    public event Action<FishingState> OnStateChanged;

    private void Start()
    {
        // Find player stats if not assigned
        if (playerStats == null)
        {
            playerStats = GetComponent<PlayerStatsController>();
            if (playerStats == null)
            {
                Debug.LogError("PlayerStatsController not found! Attach to the same GameObject or assign in inspector.");
            }
        }

        // Hide UI elements initially
        if (castingUI) castingUI.SetActive(false);
        if (bitingIndicator) bitingIndicator.SetActive(false);
        if (tensionMeter) tensionMeter.SetActive(false);

        // Hide line initially
        if (fishingLine) fishingLine.enabled = false;

        UpdateState(FishingState.Idle);
    }

    private void Update()
    {
        // Handle input based on current state
        HandleInput();

        // Update current state logic
        switch (currentState)
        {
            case FishingState.Idle:
                // Nothing to do in idle state
                break;

            case FishingState.Aiming:
                HandleAiming();
                break;

            case FishingState.Casting:
                // Casting is handled by physics after the initial force is applied
                break;

            case FishingState.Waiting:
                UpdateFishingLine();
                break;

            case FishingState.FishInterested:
                UpdateFishingLine();
                break;

            case FishingState.Biting:
                UpdateBiting();
                UpdateFishingLine();
                break;

            case FishingState.Hooked:
            case FishingState.Reeling:
                HandleReeling();
                UpdateFishingLine();
                break;

            case FishingState.Caught:
            case FishingState.Failed:
                // These states are transient and are handled in their entry methods
                break;
        }
    }

    private void HandleInput()
    {
        // Process input based on current state
        switch (currentState)
        {
            case FishingState.Idle:
                // Start aiming if cast button is pressed
                if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space))
                {
                    UpdateState(FishingState.Aiming);
                }
                break;

            case FishingState.Aiming:
                // Track if cast button is held
                castButtonHeld = Input.GetMouseButton(0) || Input.GetKey(KeyCode.Space);

                // Release cast if button is released
                if (!castButtonHeld && currentCastPower > 0.1f)
                {
                    PerformCast();
                }

                // Cancel cast if ESC is pressed
                if (Input.GetKeyDown(KeyCode.Escape))
                {
                    CancelCast();
                }
                break;

            case FishingState.Waiting:
            case FishingState.FishInterested:
                // Start reeling if reel button is pressed
                if (Input.GetMouseButtonDown(1) || Input.GetKeyDown(KeyCode.R))
                {
                    StartReeling();
                }
                break;

            case FishingState.Biting:
                // Hook the fish if player presses at the right time
                if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space))
                {
                    AttemptHook();
                }

                // Start reeling if reel button is pressed (will lose the fish)
                if (Input.GetMouseButtonDown(1) || Input.GetKeyDown(KeyCode.R))
                {
                    StartReeling();
                }
                break;

            case FishingState.Hooked:
            case FishingState.Reeling:
                // Track if reel button is held
                reelButtonHeld = Input.GetMouseButton(1) || Input.GetKey(KeyCode.R);

                // Allow toggling between hooked and reeling states
                if (reelButtonHeld && currentState == FishingState.Hooked)
                {
                    UpdateState(FishingState.Reeling);
                }
                else if (!reelButtonHeld && currentState == FishingState.Reeling)
                {
                    UpdateState(FishingState.Hooked);
                }

                // Allow cancelling by cutting the line
                if (Input.GetKeyDown(KeyCode.Escape))
                {
                    CutLine();
                }
                break;
        }
    }

    private void HandleAiming()
    {
        if (castButtonHeld)
        {
            // Increase cast power while button is held
            currentCastPower = Mathf.Min(currentCastPower + castPowerBuildupRate * Time.deltaTime, maxCastPower);

            // Calculate aim direction from mouse position
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePos.z = 0;

            Vector2 aimTarget = mousePos - transform.position;

            // Clamp angle if needed
            float angle = Vector2.SignedAngle(Vector2.right, aimTarget);
            angle = Mathf.Clamp(angle, -maxAimAngle, maxAimAngle);

            // Convert back to direction
            aimDirection = Quaternion.Euler(0, 0, angle) * Vector2.right;
            aimDirection.Normalize();

            // Update UI
            UpdateCastingUI();
        }
        else
        {
            // If button isn't held, gradually reduce power
            currentCastPower = Mathf.Max(0, currentCastPower - castPowerBuildupRate * Time.deltaTime);

            // Return to idle if power is 0
            if (currentCastPower <= 0)
            {
                UpdateState(FishingState.Idle);
            }
        }
    }

    private void PerformCast()
    {
        if (lurePrefab == null)
        {
            Debug.LogError("Lure prefab not assigned!");
            return;
        }

        // Calculate cast power based on player stats
        float effectiveCastPower = currentCastPower * playerStats.CastDistance / 10f;

        // Add some randomness based on accuracy
        float accuracyFactor = 1f / playerStats.CastAccuracy;
        Vector2 castDirection = aimDirection;
        castDirection += new Vector2(
            UnityEngine.Random.Range(-0.1f, 0.1f) * accuracyFactor,
            UnityEngine.Random.Range(-0.1f, 0.1f) * accuracyFactor
        );
        castDirection.Normalize();

        // Instantiate lure
        Vector3 castPosition = castPoint != null ? castPoint.position : transform.position;
        activeLure = Instantiate(lurePrefab, castPosition, Quaternion.identity);

        // Get lure component and set up properties
        lureComponent = activeLure.GetComponent<Lure>();
        if (lureComponent == null)
        {
            lureComponent = activeLure.AddComponent<Lure>();
        }

        // Configure lure based on player stats
        lureComponent.attractionMultiplier = playerStats.LureAttraction;

        // Add physics components for casting if they don't exist
        Rigidbody2D lureRb = activeLure.GetComponent<Rigidbody2D>();
        if (lureRb == null)
        {
            lureRb = activeLure.AddComponent<Rigidbody2D>();
            lureRb.linearDamping = 1f;
            lureRb.angularDamping = 1f;
            lureRb.gravityScale = 1f;
        }

        // Apply force in cast direction
        lureRb.AddForce(castDirection * effectiveCastPower, ForceMode2D.Impulse);

        // Subscribe to lure events
        lureComponent.OnFishInterested += HandleFishInterested;
        lureComponent.OnFishBiting += HandleFishBiting;

        // Update the fishing line
        if (fishingLine)
        {
            fishingLine.enabled = true;
            UpdateFishingLine();
        }

        // Update state
        UpdateState(FishingState.Casting);

        // Start coroutine to transition to waiting state after lure settles
        StartCoroutine(WaitForLureToSettle(lureRb));
    }

    private IEnumerator WaitForLureToSettle(Rigidbody2D lureRb)
    {
        // Wait for lure to settle or hit water
        float settleTime = 2f; // Timeout to prevent hanging
        float waitTime = 0f;

        while (waitTime < settleTime && lureRb.linearVelocity.magnitude > 0.1f)
        {
            waitTime += Time.deltaTime;
            yield return null;
        }

        // Make lure sink slower once it hits water
        lureRb.gravityScale = 0.5f;
        lureRb.linearDamping = 3f;

        // Transition to waiting state
        UpdateState(FishingState.Waiting);
    }

    private void HandleFishInterested(FishAI fish)
    {
        if (currentState == FishingState.Waiting)
        {
            // Fish is interested but not biting yet
            UpdateState(FishingState.FishInterested);

            // Play subtle indication effect here
            Debug.Log($"Fish is interested in lure: {fish.fishType.speciesID}");
        }
    }

    private void HandleFishBiting(FishAI fish)
    {
        if (currentState == FishingState.Waiting || currentState == FishingState.FishInterested)
        {
            // Fish is biting - player needs to hook it
            hookedFish = fish;
            biteTimer = biteDuration;

            // Transition to biting state
            UpdateState(FishingState.Biting);

            // Activate biting indicator
            if (bitingIndicator) bitingIndicator.SetActive(true);

            Debug.Log($"Fish is biting: {fish.fishType.speciesID}");
        }
    }

    private void UpdateBiting()
    {
        // Count down bite timer
        biteTimer -= Time.deltaTime;

        // If player doesn't hook in time, fish escapes
        if (biteTimer <= 0)
        {
            // Fish lost interest
            Debug.Log("Fish lost interest");
            hookedFish = null;

            // Hide biting indicator
            if (bitingIndicator) bitingIndicator.SetActive(false);

            // Return to waiting state
            UpdateState(FishingState.Waiting);
        }
    }

    private void AttemptHook()
    {
        if (hookedFish != null)
        {
            // Successfully hooked the fish
            Debug.Log($"Fish hooked: {hookedFish.fishType.speciesID}");

            // Hide biting indicator
            if (bitingIndicator) bitingIndicator.SetActive(false);

            // Show tension meter
            if (tensionMeter) tensionMeter.SetActive(true);

            // Set lure as occupied
            if (lureComponent) lureComponent.isOccupied = true;

            // Transition to hooked state
            UpdateState(FishingState.Hooked);

            // Tell the fish it's been hooked
            if (hookedFish.gameObject.CompareTag("Fish"))
            {
                hookedFish.gameObject.tag = "OccupiedLure";
            }
        }
    }

    private void StartReeling()
    {
        // If there's a fish on the line, it will escape
        if (hookedFish != null && (currentState == FishingState.Biting || currentState == FishingState.FishInterested))
        {
            Debug.Log("Fish escaped while reeling");
            hookedFish = null;
        }

        // Hide UI elements
        if (bitingIndicator) bitingIndicator.SetActive(false);

        // Start reeling in the lure
        UpdateState(FishingState.Reeling);
    }

    private void HandleReeling()
    {
        if (activeLure == null) return;

        // Calculate reel speed
        float reelSpeed = playerStats.ReelSpeed * (reelButtonHeld ? 1f : 0.3f);

        // Calculate direction to player
        Vector2 lurePos = activeLure.transform.position;
        Vector2 playerPos = transform.position;
        Vector2 dirToPlayer = (playerPos - lurePos).normalized;

        // Move lure toward player
        activeLure.transform.position = Vector2.MoveTowards(
            lurePos,
            playerPos,
            reelSpeed * Time.deltaTime
        );

        // If hooked, update tension meter
        if (hookedFish != null)
        {
            // Calculate tension based on fish strength and player stats
            float fishStrength = hookedFish.fishData != null ?
                hookedFish.fishData.forceMultiplier : 1f;

            // Tension increases when reeling, decreases when not
            if (reelButtonHeld)
            {
                currentTension += fishStrength * Time.deltaTime * 10f;
                currentTension = Mathf.Min(currentTension, maxTension);
            }
            else
            {
                currentTension -= playerStats.TensionResistance * Time.deltaTime * 5f;
                currentTension = Mathf.Max(currentTension, 0f);
            }

            // Update tension meter UI
            UpdateTensionUI();

            // Check if line breaks due to tension
            if (currentTension >= maxTension)
            {
                LineBroke();
                return;
            }

            // Move the fish with the lure
            hookedFish.transform.position = activeLure.transform.position;
        }

        // Check if lure is close enough to count as caught
        float distanceToPlayer = Vector2.Distance(lurePos, playerPos);
        if (distanceToPlayer <= minCatchDistance)
        {
            // If fish is on the hook, it's caught
            if (hookedFish != null)
            {
                FishCaught();
            }
            else
            {
                // Just reeled in an empty lure
                ReelComplete();
            }
        }
    }

    private void FishCaught()
    {
        if (hookedFish == null) return;

        Debug.Log($"Fish caught: {hookedFish.fishType.speciesID}");

        // Trigger catch event
        OnFishCaught?.Invoke(hookedFish);

        // Clean up
        CleanupFishing();

        // Update state
        UpdateState(FishingState.Caught);

        // Return to idle state after a short delay
        StartCoroutine(ReturnToIdleAfterDelay(1f));
    }

    private void LineBroke()
    {
        Debug.Log("Line broke due to tension!");

        // Clean up
        CleanupFishing();

        // Update state
        UpdateState(FishingState.Failed);

        // Return to idle state after a short delay
        StartCoroutine(ReturnToIdleAfterDelay(1f));
    }

    private void CutLine()
    {
        Debug.Log("Line cut manually");

        // Clean up
        CleanupFishing();

        // Update state
        UpdateState(FishingState.Failed);

        // Return to idle state after a short delay
        StartCoroutine(ReturnToIdleAfterDelay(0.5f));
    }

    private void ReelComplete()
    {
        Debug.Log("Lure reeled in successfully");

        // Clean up
        CleanupFishing();

        // Update state
        UpdateState(FishingState.Idle);
    }

    private void CancelCast()
    {
        // Reset state
        currentCastPower = 0f;

        // Hide UI
        if (castingUI) castingUI.SetActive(false);

        // Return to idle state
        UpdateState(FishingState.Idle);
    }

    private void CleanupFishing()
    {
        // Unsubscribe from lure events
        if (lureComponent != null)
        {
            lureComponent.OnFishInterested -= HandleFishInterested;
            lureComponent.OnFishBiting -= HandleFishBiting;
        }

        // Destroy lure
        if (activeLure != null)
        {
            Destroy(activeLure);
            activeLure = null;
        }

        // Reset hooked fish
        hookedFish = null;

        // Hide UI elements
        if (bitingIndicator) bitingIndicator.SetActive(false);
        if (tensionMeter) tensionMeter.SetActive(false);

        // Hide fishing line
        if (fishingLine) fishingLine.enabled = false;

        // Reset tension
        currentTension = 0f;
    }

    private IEnumerator ReturnToIdleAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        UpdateState(FishingState.Idle);
    }

    private void UpdateFishingLine()
    {
        if (fishingLine && activeLure)
        {
            Vector3 startPos = castPoint != null ? castPoint.position : transform.position;
            Vector3 endPos = activeLure.transform.position;

            fishingLine.SetPosition(0, startPos);
            fishingLine.SetPosition(1, endPos);
        }
    }

    private void UpdateCastingUI()
    {
        // Update casting UI if available
        if (castingUI)
        {
            castingUI.SetActive(true);

            // Update power indicator and direction indicator
            // This would depend on your specific UI implementation
        }
    }

    private void UpdateTensionUI()
    {
        // Update tension UI if available
        if (tensionMeter)
        {
            // This would depend on your specific UI implementation
            // For example, setting the fill amount of an image
        }
    }

    private void UpdateState(FishingState newState)
    {
        if (currentState == newState) return;

        FishingState oldState = currentState;
        currentState = newState;

        // Handle exit actions for old state
        switch (oldState)
        {
            case FishingState.Aiming:
                if (castingUI) castingUI.SetActive(false);
                break;

            case FishingState.Biting:
                if (bitingIndicator) bitingIndicator.SetActive(false);
                break;

            case FishingState.Hooked:
            case FishingState.Reeling:
                if (tensionMeter) tensionMeter.SetActive(false);
                break;
        }

        // Handle entry actions for new state
        switch (newState)
        {
            case FishingState.Idle:
                currentCastPower = 0f;
                currentTension = 0f;
                break;

            case FishingState.Aiming:
                currentCastPower = 0f;
                if (castingUI) castingUI.SetActive(true);
                break;

            case FishingState.Biting:
                if (bitingIndicator) bitingIndicator.SetActive(true);
                break;

            case FishingState.Hooked:
            case FishingState.Reeling:
                if (tensionMeter) tensionMeter.SetActive(true);
                break;
        }

        // Trigger state change event
        OnStateChanged?.Invoke(newState);

        Debug.Log($"Fishing state changed: {oldState} -> {newState}");
    }
}