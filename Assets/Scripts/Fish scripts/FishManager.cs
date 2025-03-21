using NUnit.Framework;
using UnityEngine;

public class FishManager : MonoBehaviour
{
    //Reference to the FishSpawner component
    public FishSpawner fishSpawner;

    //all fish in scene
    //public List<FishAI> allFish = new List<FishAI>();

    private void Start()
    {
        foreach (FishType fishType in fishSpawner.fishTypes)
        {
            
            SpawnRegion region = fishType.spawnRegion != null
                ? fishType.spawnRegion
                : fishSpawner.GetSpawnRegionForSpecies(fishType.speciesID);


            if (region != null)
                fishSpawner.SpawnSchool(fishType, region);
            else
                Debug.Log($"No spawn region found for {fishType.typeName}");
            
        }
    }
}
