using UnityEngine;
using UnityEngine.UI;

public class HookedState : StateInterface
{
    private GameObject lure;
    private GameObject hookedFish;
    private Rigidbody2D fishRb;
    private float struggleForce = 10f;
    private float playerForce = 50f;

    private GameObject tensionBarGameObject;
    private Image tensionBarMask;
    private float currentTension = 0f;
    private float maxTension = 100f;
    private float tensionDecreaseRate = 5f;
    private float tensionIncreaseRate = 20f;

    private float fishStruggleInterval = 1.5f;
    private float nextStruggleTime = 0f;


    private float captureDistance = 2f;

    public HookedState(GameObject tensionBarGameObject = null)
    {
        this.tensionBarGameObject = tensionBarGameObject;
    }

    public void Enter()
    {
        lure = GameObject.FindWithTag("Lure");
        if (lure == null)
        {
            Debug.LogError("No lure found in HookedState");
            return;
        }

        // Find the hooked fish by checking for fish near the lure
        FindHookedFish();

        SetupTensionBar();

        // Disable fish AI behavior while hooked
        if (hookedFish != null)
        {
            FishAI fishAI = hookedFish.GetComponent<FishAI>();
            if (fishAI != null)
            {
                fishAI.enabled = false;
            }

            fishRb = hookedFish.GetComponent<Rigidbody2D>();
            if (fishRb == null)
            {
                Debug.LogError("Hooked fish has no Rigidbody2D component");
            }

            // Mark the lure as occupied with this fish
            LureStateController lureState = lure.GetComponent<LureStateController>();
            if (lureState == null)
            {
                // Add the component if it doesn't exist
                lureState = lure.AddComponent<LureStateController>();
            }
            lureState.SetOccupied(hookedFish);
        }
    }

    public void Update()
    {
        if (hookedFish == null || lure == null) return;

        // Make fish struggle (rotate and apply force away from player)
        ApplyFishStruggle();

        // Handle player input to reel in the fish
        HandlePlayerInput();

        // Update tension bar
        UpdateTensionBar();

        // Check if fish is caught or escaped
        CheckFishStatus();
    }

    public void Exit()
    {
        if (tensionBarGameObject != null)
        {
            tensionBarGameObject.SetActive(false);
        }
    }

    private void FindHookedFish()
    {
        // Find the closest fish to the lure
        Collider2D[] nearbyColliders = Physics2D.OverlapCircleAll(lure.transform.position, 1f);

        foreach (Collider2D collider in nearbyColliders)
        {
            if (collider.gameObject.GetComponent<FishAI>() != null)
            {
                hookedFish = collider.gameObject;
              
                // Attach the lure to the fish using a fixed joint
                AttachLureToFish();
                break;
            }
        }

        if (hookedFish == null)
        {
            Debug.LogError("No fish found near lure in HookedState");
        }
    }

    // Method to physically attach lure to the fish
    private void AttachLureToFish()
    {
        if (hookedFish == null || lure == null) return;

        // Get hook position
        Vector2 hookPosition = hookedFish.transform.position;


        // Create a fixed joint between the fish and the lure
        FixedJoint2D joint = hookedFish.AddComponent<FixedJoint2D>();
        joint.connectedBody = lure.GetComponent<Rigidbody2D>();
        joint.autoConfigureConnectedAnchor = false;
        joint.anchor = Vector2.zero; // Center of the fish

        // Disable the lure's collider to prevent further fish interactions
        Collider2D lureCollider = lure.GetComponent<Collider2D>();
        if (lureCollider != null)
        {
            lureCollider.enabled = false;
        }
    }

    private void SetupTensionBar()
    {
        if (tensionBarGameObject != null)
        {
            tensionBarGameObject.SetActive(true);

            Transform maskTransform = tensionBarGameObject.transform.Find("Mask");
            if (maskTransform != null)
            {
                tensionBarMask = maskTransform.GetComponent<Image>();
                currentTension = 0f;

            }

            if (tensionBarMask == null)
            {
                Debug.LogError("Tension bar mask image not found");
            }
        }
        else
        {
            Debug.Log("No tension bar assigned to HookedState");
        }
    }

    private void ApplyFishStruggle()
    {
        if (fishRb == null) return;

        // Find player position
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null) return;

        // Calculate direction away from player
        Vector2 awayFromPlayer = (hookedFish.transform.position - player.transform.position).normalized;

        // Rotate fish to face away from player (add 180 degrees to face away)
        float angle = Mathf.Atan2(awayFromPlayer.y, awayFromPlayer.x) * Mathf.Rad2Deg;
        hookedFish.transform.rotation = Quaternion.Euler(0, 0, angle);

        // Apply force to fish to swim away from player
        fishRb.AddForce(awayFromPlayer * struggleForce * Time.deltaTime, ForceMode2D.Impulse);

        // Decrease tension as fish struggles
        currentTension -= tensionDecreaseRate * Time.deltaTime;
    }

    private void HandlePlayerInput()
    {
        // When space is pressed, pull the fish toward the player
        if (Input.GetKey(KeyCode.Space))
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player == null || fishRb == null) return;

            Vector2 towardPlayer = (player.transform.position - hookedFish.transform.position).normalized;
            fishRb.AddForce(towardPlayer * playerForce * Time.deltaTime, ForceMode2D.Impulse);

            // Increase tension when reeling
            currentTension += tensionIncreaseRate * Time.deltaTime;
        }
    }

    private void UpdateTensionBar()
    {
        // Clamp tension value
        currentTension = Mathf.Clamp(currentTension, 0, maxTension);

        // Update UI if available
        if (tensionBarMask != null)
        {
            float fillAmount = currentTension / maxTension;     
            tensionBarMask.fillAmount = fillAmount;     
        }
    }
    
    private void CheckFishStatus()
    {
        // Check if tension too high
        if (currentTension >= maxTension)
        {
            //Debug.Log("Fish escaped - tension too high!");
            // Fish escapes - reset and go back to passive state
            GameObject stateControllerObj = GameObject.FindWithTag("GameController");
            Debug.Log(stateControllerObj);
            if (stateControllerObj != null)
            {
                StateController controller = stateControllerObj.GetComponent<StateController>();
                if (controller != null)
                {                 
                    controller.FishEscaped();
                    return;
                }
            }
        }

        // Check if fish is close enough to player to be caught
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null && hookedFish != null)
        {
            float distanceToPlayer = Vector2.Distance(player.transform.position, hookedFish.transform.position);
            if (distanceToPlayer < captureDistance)
            {
                //Debug.Log("Fish caught successfully!");
                
                // Get FishType for potential inventory/rewards
                FishAI fishAI = hookedFish.GetComponent<FishAI>();
                if (fishAI != null && fishAI.fishType != null)
                {
                   Debug.Log($"Caught fish of type: {fishAI.fishType.speciesID}");
                }

                // Return to passive state
                GameObject stateControllerObj = GameObject.FindWithTag("GameController");
                if (stateControllerObj != null)
                {
                    StateController controller = stateControllerObj.GetComponent<StateController>();
                    if (controller != null)
                    {
                        controller.FishCaught(hookedFish);
                    }
                }
            }
        }
    }
}