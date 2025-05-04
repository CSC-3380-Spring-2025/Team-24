using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// Represents a fishing lure in the game world that can attract and catch fish
/// </summary>
public class Lure : MonoBehaviour
{
    [Header("Lure Properties")]
    public float attractionMultiplier = 1f;
    public float visibilityMultiplier = 1f;
    public float sizeMultiplier = 1f;

    [Header("Detection Settings")]
    [SerializeField] private float interestRadius = 8f;
    [SerializeField] private float biteRadius = 2f;
    [SerializeField] private float baseBiteChance = 0.3f;
    [SerializeField] private float minBiteTime = 2f;
    [SerializeField] private float maxBiteTime = 10f;

    // State tracking
    [HideInInspector] public bool isOccupied = false;
    private bool isInWater = false;
    private bool isFishInterested = false;
    private FishAI interestedFish = null;
    private Coroutine biteCoroutine = null;

    // Events
    public event Action<FishAI> OnFishInterested;
    public event Action<FishAI> OnFishBiting;

    // Static events for integration with other systems
    public static event Action<Transform> OnLureCreated;
    public static event Action OnLureDestroyed;

    private void Start()
    {
        // Notify other systems (like line drawer) about this lure
        OnLureCreated?.Invoke(transform);
    }

    private void OnDestroy()
    {
        // Notify other systems when lure is destroyed
        OnLureDestroyed?.Invoke();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Detect when lure enters water
        if (other.CompareTag("Water") && !isInWater)
        {
            isInWater = true;

            // Adjust physics for underwater movement
            Rigidbody2D rb = GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.linearDamping = 3f;
                rb.gravityScale = 0.5f;
            }

            Debug.Log("Lure entered water");
        }
    }

    private void Update()
    {
        if (!isInWater || isOccupied) return;

        // Only detect fish if the lure is in water and not already occupied
        DetectFish();
    }

    private void DetectFish()
    {
        if (isFishInterested && interestedFish != null)
        {
            // Check if interested fish is still within bite radius
            float distanceToFish = Vector2.Distance(transform.position, interestedFish.transform.position);

            // If fish moves away, lose interest
            if (distanceToFish > interestRadius)
            {
                ResetFishInterest();
            }

            return;
        }

        // Look for nearby fish that might be interested
        Collider2D[] nearby = Physics2D.OverlapCircleAll(transform.position, interestRadius);

        foreach (Collider2D collider in nearby)
        {
            if (collider.CompareTag("Fish"))
            {
                FishAI fish = collider.GetComponent<FishAI>();

                if (fish != null)
                {
                    // Check if fish is in sight range
                    float distanceToFish = Vector2.Distance(transform.position, fish.transform.position);

                    // Calculate fish interest based on lure properties and fish type
                    float interestFactor = CalculateFishInterest(fish);

                    // If fish is interested
                    if (interestFactor > 0.5f && !isFishInterested)
                    {
                        // Set as interested fish
                        interestedFish = fish;
                        isFishInterested = true;

                        // Notify about fish interest
                        OnFishInterested?.Invoke(fish);

                        // Start coroutine to check if fish will bite
                        biteCoroutine = StartCoroutine(CheckForBite(fish));

                        Debug.Log($"Fish {fish.fishType.speciesID} is interested in lure");

                        // Only handle one interested fish at a time
                        break;
                    }
                }
            }
        }
    }

    private float CalculateFishInterest(FishAI fish)
    {
        // Base interest level
        float interest = 0.5f;

        // Apply lure attraction multiplier
        interest *= attractionMultiplier;

        // Distance factor - closer fish are more interested
        float distanceToFish = Vector2.Distance(transform.position, fish.transform.position);
        float distanceFactor = 1f - Mathf.Clamp01(distanceToFish / interestRadius);
        interest *= distanceFactor;

        // Optional: Apply fish-specific interest factors
        // For example, certain fish might be more interested in certain lure types
        // This could be implemented by checking fish.fishType and lure properties

        // Size compatibility - some fish prefer lures of certain sizes
        float fishSize = fish.transform.localScale.x;
        float sizeDifference = Mathf.Abs(fishSize - sizeMultiplier);
        float sizeCompatibility = 1f - Mathf.Clamp01(sizeDifference);
        interest *= (0.5f + sizeCompatibility * 0.5f); // Size affects interest but isn't the only factor

        return interest;
    }

    private IEnumerator CheckForBite(FishAI fish)
    {
        if (fish == null)
        {
            ResetFishInterest();
            yield break;
        }

        // Wait a random amount of time before the fish considers biting
        float timeUntilBite = UnityEngine.Random.Range(minBiteTime, maxBiteTime);
        float elapsedTime = 0f;

        while (elapsedTime < timeUntilBite)
        {
            // Check if fish is still close enough
            float distanceToFish = Vector2.Distance(transform.position, fish.transform.position);

            if (distanceToFish > biteRadius || !isFishInterested || fish == null)
            {
                // Fish lost interest or moved away
                ResetFishInterest();
                yield break;
            }

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Decide if fish will bite based on chance
        float biteChance = CalculateBiteChance(fish);

        if (UnityEngine.Random.value < biteChance)
        {
            // Fish bites!
            OnFishBiting?.Invoke(fish);
            Debug.Log($"Fish {fish.fishType.speciesID} is biting the lure!");
        }
        else
        {
            // Fish didn't bite this time, but might stay interested
            float stayInterestedChance = 0.4f;

            if (UnityEngine.Random.value < stayInterestedChance)
            {
                // Try again
                biteCoroutine = StartCoroutine(CheckForBite(fish));
            }
            else
            {
                // Fish lost interest
                ResetFishInterest();
            }
        }
    }

    private float CalculateBiteChance(FishAI fish)
    {
        // Start with base bite chance
        float chance = baseBiteChance;

        // Modify by lure attraction
        chance *= attractionMultiplier;

        // Size compatibility affects bite chance
        float fishSize = fish.transform.localScale.x;
        float sizeDifference = Mathf.Abs(fishSize - sizeMultiplier);
        float sizeCompatibility = 1f - Mathf.Clamp01(sizeDifference);
        chance *= (0.5f + sizeCompatibility * 0.5f);

        // Deeper fish are generally more cautious
        float depth = 0f;
        StateController stateController = FindObjectOfType<StateController>();
        if (stateController != null)
        {
            depth = stateController.waterLevel - fish.transform.position.y;
            float depthFactor = Mathf.Clamp01(depth / 10f); // Deeper = more cautious
            chance *= (1f - depthFactor * 0.3f); // Reduce chance by up to 30% for deep fish
        }

        // Consider fish hunger level
        chance *= (0.7f + fish.hunger * 0.3f); // Hungry fish are more likely to bite

        // Clamp final chance
        return Mathf.Clamp01(chance);
    }

    private void ResetFishInterest()
    {
        isFishInterested = false;
        interestedFish = null;

        if (biteCoroutine != null)
        {
            StopCoroutine(biteCoroutine);
            biteCoroutine = null;
        }
    }

    private void OnDrawGizmosSelected()
    {
        // Draw interest radius
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interestRadius);

        // Draw bite radius
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, biteRadius);
    }
}