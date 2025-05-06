using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class FishManager : MonoBehaviour
{
    public class PopulationData
    {
        public FishType fishType; //Type of fish
        public int currentPopulation; //Current population of the fish type
        public float lastSpawnTime = 0f; //Last time a fish of this type was spawned
        public List<FishAI> activeFish = new List<FishAI>(); //List of active fish of this type
    }

    [Header("Ecosystem Settings")]
    public List<FishType> managedFishTypes = new List<FishType>(); //List of fish types to manage
    public List<SpawnRegion> spawnRegions = new List<SpawnRegion>(); //List of population data for each fish type
    public float populationUpdateInterval = 60f; //Interval for updating the population data in seconds
    public float timeScale = 1f; //Scale for time, used to speed up or slow down the simulation

    [Header("Debug")]
    public bool showPopulationStats = true;

    [Header("Population Data")]
    public Dictionary<string, PopulationData> populations = new Dictionary<string, PopulationData>(); //Dictionary to store population data for each fish type
    public float lastPopulationUpdateTime = 0f; //Last time the population data was updated
    public float gameTime = 0f; //Current game time in seconds

    [Header("Fish Generator")]
    public FishGenerator fishGenerator;

    private void Start()
    {
        //InitializePopulationTracking
        foreach (FishType fishType in managedFishTypes)
        {
            PopulationData data = new PopulationData
            {
                fishType = fishType,
                currentPopulation = 0,
                lastSpawnTime = 0f,
            };

            populations[fishType.speciesID] = data; //Add the population data to the dictionary
        }

        //Initial population setup
        PerformInitialSpawning();

        //Start enviromental cycle
        
    }


    private void Update()
    {
        //Update game time with time scaling
        gameTime += Time.deltaTime * timeScale;

        //Check if it's time to update the population data
        if (gameTime - lastPopulationUpdateTime >= populationUpdateInterval)
        {
            UpdatePopulations();
            lastPopulationUpdateTime = gameTime; //Reset the last update time
        }
    }


    private void PerformInitialSpawning()
    {
        foreach (FishType fishType in managedFishTypes)
        {
            int initialCount = fishType.targetPopulation / 2; //Spawn half

            //Find suitable spawn region for the fish type
            List<SpawnRegion> suitableRegions = FindSuitableRegions(fishType);
            if (suitableRegions.Count > 0)
            {
                int fishPerRegion = initialCount / suitableRegions.Count; //Distribute fish evenly across suitable regions
                int remainder = initialCount % suitableRegions.Count;

                for (int i = 0; i < suitableRegions.Count; i++)
                {
                    int fishToSpawn = fishPerRegion;
                    if (i == 0) fishToSpawn += remainder; //Add any remainder to the first region
                    //Generate a serializable fish item
                    SerializableFishItem serializableFishItem = fishGenerator.GenerateSerializableFish(fishType, 1); //Generate a serializable fish item
                    SpawnFishGroup(fishType, suitableRegions[i], fishToSpawn, serializableFishItem);
                }
            }
        }
    }


    private List<SpawnRegion> FindSuitableRegions(FishType fishType)
    {
        List<SpawnRegion> suitable = new List<SpawnRegion>();

        foreach (SpawnRegion region in spawnRegions)
        {
            if (region.speciesID == fishType.speciesID)
            {
                suitable.Add(region);
            }
        }
        return suitable;
    }


    private void SpawnFishGroup(FishType fishType, SpawnRegion region, int count, SerializableFishItem serializableFishItem)
    {
        //Create a parent object for the school
        GameObject schoolParent = new GameObject(fishType.speciesID + " School");

        //Get population data
        PopulationData populationData = populations[fishType.speciesID];

        //Random position within region for school center
        Vector2 schoolCenter = region.GetRandomPosition();
        schoolParent.transform.position = schoolCenter;

        for (int i = 0; i < count; i++)
        {
            //Random position within the region for each fish
            Vector2 spawnPosition = schoolCenter + new Vector2(Random.Range(-2f, 2f), Random.Range(-2f, 2f));

            //Create fish instance
            //Get the FishGenerator component from the FishManager
            if (fishGenerator == null)
            {
                Debug.LogError("FishGenerator component not found on FishManager. Please add it.");
                return;
            }

            
            GameObject fishObj = Instantiate(fishType.prefab, spawnPosition, Quaternion.identity); //Instantiate the fish prefab
            fishObj.transform.parent = schoolParent.transform; //Set the parent to the school object

            //Set up fish AI
            FishAI fishAI = fishObj.GetComponent<FishAI>();
            if (fishAI == null)
            {
                fishAI = fishObj.AddComponent<FishAI>(); //Add FishAI component if not present
            }

            fishAI.Initialize(fishType, region, schoolCenter, serializableFishItem); //Initialize fish AI 

            //Subscribe to fish events
            fishAI.OnFishDeath += HandleFishDeath; //Subscribe to the death event
            fishAI.OnFishReproduce += HandleFishReproduction; //Subscribe to the reproduction event

            populationData.activeFish.Add(fishAI); //Add fish to the active list
            populationData.currentPopulation++; //Increment the current population

        }
        populationData.lastSpawnTime = gameTime; //Update the last spawn time for this fish type
    }


    private void UpdatePopulations()
    {
        foreach (var entry in populations)
        {
            PopulationData data = entry.Value;
            FishType fishType = data.fishType;

            data.activeFish.RemoveAll(fish => fish == null); //Remove any null fish references

            data.currentPopulation = data.activeFish.Count; //Update the current population count

            //Check if we need to spawn more fish
            if (data.currentPopulation < fishType.targetPopulation)
            {
                //Calculate spawn probability based n time since last spawn
                float timeSinceLastSpawn = gameTime - data.lastSpawnTime;
                float spawnChance = timeSinceLastSpawn * fishType.spawnRate * timeScale; //Adjust spawn chance

                if (Random.value < spawnChance)
                {
                    int fishToSpawn = Mathf.Min(
                        Random.Range(1, 3), // //Randomly spawn 1-2 fish
                        fishType.targetPopulation - data.currentPopulation //Ensure we don't exceed target population
                    );

                    if (fishToSpawn > 0)
                    {
                        //Find suitable regions for spawning
                        List<SpawnRegion> suitableRegions = FindSuitableRegions(fishType); //Find suitable regions for spawning
                        if (suitableRegions.Count > 0)
                        {
                            SpawnRegion region = suitableRegions[Random.Range(0, suitableRegions.Count)]; //Select a random suitable region
                            //Generate a serializable fish item
                            SerializableFishItem serializableFishItem = fishGenerator.GenerateSerializableFish(fishType, 1); //Generate a serializable fish item
                            SpawnFishGroup(fishType, region, fishToSpawn, serializableFishItem);
                        }
                    }
                }
            }
        }   
    }


    private void HandleFishDeath(FishAI fish)
    {
        if (fish == null || fish.fishType == null) return; //Exit if fish is null or fish type is not set

        string speciesID = fish.fishType.speciesID; //Get the species ID of the fish
        if (populations.TryGetValue(speciesID, out PopulationData populationData))
        {
            populationData.activeFish.Remove(fish); //Remove fish from the active list
            populationData.currentPopulation--; //Decrement the current population
        }
    }


    private void HandleFishReproduction(FishAI parent)
    {
        if (parent == null) return;

        string speciesID = parent.fishType.speciesID; //Get the species ID of the parent fish
        if (populations.TryGetValue(speciesID, out PopulationData populationData))
        {
            if (populationData.currentPopulation >= parent.fishType.maxPopulation)
            {
                //If the population is at max, don't allow reproduction
                return;
            }
            Vector2 parentPosition2D = new Vector2(parent.transform.position.x, parent.transform.position.y);
            Vector2 spawnPos = parentPosition2D + new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)); //Random position around the parent fish
            GameObject babyFishObj = Instantiate(parent.fishType.prefab, spawnPos, Quaternion.identity); //Create a new fish object
   
            //Set up fish AI for the baby fish
            FishAI babyFishAI = babyFishObj.GetComponent<FishAI>();
            if (babyFishAI == null)
            {
                babyFishAI = babyFishObj.AddComponent<FishAI>(); //Add FishAI component if not present
            }

            babyFishAI.Initialize(parent.fishType, parent.currentRegion, parent.homePosition); //Initialize baby fish AI
            babyFishAI.OnFishDeath += HandleFishDeath; //Subscribe to the death event
            babyFishAI.OnFishReproduce += HandleFishReproduction; //Subscribe to the reproduction event

            babyFishAI.age = 0f; //Set the age of the baby fish to 0
            babyFishAI.transform.localScale = Vector3.one * 0.5f; //Scale down the baby fish

            //Track this fish in our populaiton
            populationData.activeFish.Add(babyFishAI); //Add baby fish to the active list
            populationData.currentPopulation++; //Increment the current population

        }
    }


    private void OnGUI()
    {
        if(!showPopulationStats) return; //Exit if we don't want to show stats

        int y = 10;
        
        foreach (var entry in populations)
        {
            GUI.Label(new Rect(10, y, 300, 20), $"{entry.Value.fishType.speciesID}: {entry.Value.currentPopulation}/{entry.Value.fishType.targetPopulation} fish");
            y += 20; //Move down for the next label
        }
    }
}
