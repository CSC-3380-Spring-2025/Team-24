using UnityEngine;

[CreateAssetMenu(fileName = "NewFishType", menuName = "Fish/FishType")]
public class FishType : ScriptableObject
{
    [Header("Basic information")]
    public string speciesID;
    public GameObject prefab; //Prefab for the fish
    public float spawnWeight;
    public int BestiaryID;

    [Header("Spawning properties")]
    public SpawnRegion spawnRegion;
    public int schoolSizeMin = 3; //Minimum size of a school to spawn
    public int schoolSizeMax = 10; //Maximum size of a school to spawn

    [Header("Population Control")]
    public int targetPopulation = 20; //Target population for the species
    public int maxPopulation = 20; //Maximum population for the species
    public float spawnRate = 0.1f;  //New fish per minute 
    public float naturalDeathRate = 0.05f; //Chance of fish dying per minute

    [Header("Environmental properties")]
    public float preferredDepthMin = 2f; 
    public float preferredDepthMax = 10f;
    [Tooltip("Minimum depth for lighter fish (lower weight)")]
    public float lightWeightDepthMin = 3f;  // Increased to keep small fish away from surface
    [Tooltip("Maximum depth for lighter fish (lower weight)")]
    public float lightWeightDepthMax = 7f;  // Extended range for small fish
    [Tooltip("Minimum depth for medium weight fish")]
    public float mediumWeightDepthMin = 6f;  // Increased to keep medium fish in middle depths
    [Tooltip("Maximum depth for medium weight fish")]
    public float mediumWeightDepthMax = 12f; // Extended range for medium fish
    [Tooltip("Minimum depth for heavier fish (higher weight)")]
    public float heavyWeightDepthMin = 10f; // Significantly deeper for large fish
    [Tooltip("Maximum depth for heavier fish (higher weight)")]
    public float heavyWeightDepthMax = 18f; // Extended depth for large fish

    [Header("lifecycle")]
    public float growthRate = 0.1f; //Size increase per min
    public float maturityAge = 5f; //Mins until reproduction maturity
    public float maxAge = 30f; //Minutes until maximum lifespan
    public float reproductionRate = 0.02f; //Chance per minute for mature fish to reproduce

    [Header("Feeding Behavior")]
    public float hungerRate = 0.1f; //Rate at which fish get hungry

    [Header("AI Parameters")]
    public float maxSpeed = 2f;
    public float maxForce = 0.5f;
    public float neighborRadius = 3f;
    public float separationDistance = 3f;
    public float lureAttractionRadius = 5f; //Radius for lure attraction, fish will be attracted to lures within this radius
    public float alignmentWeight = 1f;
    public float cohesionWeight = 0.5f;
    public float separationWeight = 1f;
    public float wanderWeight = 1f;
    public float homeAttractionWeight = 0.03f; // Greatly reduced to allow fish to roam across the entire scene
    [Range(0, 360)]
    public float fieldOfView = 270f; //Field of view for the fish AI
}
