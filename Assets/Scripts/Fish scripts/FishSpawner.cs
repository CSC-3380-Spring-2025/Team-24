using System;
using System.Collections.Generic;
using NUnit.Framework;
using Unity.Jobs;
using UnityEngine;

//FishSpawner is responsible for Spawning Fish
public class FishSpawner : MonoBehaviour
{
    public List<FishType> fishTypes;
    public float spawnFromCenterRad;
   

    //SpawnSchool takes a FishType and SpawnRegion and creates a school of a random size based on the FishTypes school size range
    public List<FishAI> SpawnSchool(FishType fishType, SpawnRegion spawnRegion)
    {
        List<FishAI> school = new List<FishAI>(); //list of all Fish within school 
        int schoolSize = UnityEngine.Random.Range(fishType.schoolSizeMin, fishType.schoolSizeMax); //create random school size

        Bounds regionBounds = spawnRegion.GetBounds(); //set region bounds from spawnRegion
        Vector2 schoolCenter = new Vector2( //set random center within the bounds
            UnityEngine.Random.Range(regionBounds.min.x, regionBounds.max.x),
            UnityEngine.Random.Range(regionBounds.min.y, regionBounds.max.y)
        );

        GameObject schoolParent = new GameObject(fishType.speciesID + "School"); //Parent GameObject to collect school of fish
        schoolParent.transform.position = schoolCenter; //Set the parents transform to the random center

        for (int i = 0; i < schoolSize; i++) //Add schoolSize amount of fishObjects
        {
            //Spawn fish near center
            Vector2 spawnPos = schoolCenter + UnityEngine.Random.insideUnitCircle * spawnFromCenterRad; //Spawn of each fish position is within spawnCenter Rad circle
            GameObject fishObj = Instantiate(fishType.prefab, spawnPos, Quaternion.identity);

            fishObj.transform.parent = schoolParent.transform; //Set parent of fishObject to be the schoolParent

            FishAI fishAI = fishObj.GetComponent<FishAI>(); //attach FishAI to fishObj
            fishObj.GetComponent<Collider2D>();
            if (fishAI != null) //Set FishAI's home and type
            {
                fishAI.homePosition = schoolCenter; //prev. gen center
                fishAI.fishType = fishType; //type pased to function
            }
            school.Add(fishAI); //add to school list
        }

        return school;

    }
        

    //Looks for a Spawn region with matching speciesID
    public SpawnRegion GetSpawnRegionForSpecies(string speciesID)
    {
        GameObject[] regionObjects = GameObject.FindGameObjectsWithTag("FishBounds"); //Finds all gameobjects with FishBounds tag
        Debug.Log($"Found {regionObjects.Length} objects with tag FishBounds");

        List<SpawnRegion> regionList = new List<SpawnRegion>(); //list of SpawnRegions

        //Goes through all objects with tag "FishBounds" 
        foreach (GameObject regionObject in regionObjects)
        {
            SpawnRegion region = regionObject.GetComponent<SpawnRegion>(); //Takes spawn region component
            if (region != null) 
            {
                regionList.Add(region);
            }
        }
        
        //Goes through all valid SpawnRegions
        foreach (SpawnRegion region in regionList) { 
            if (region.speciesID == speciesID) //Finds first occurance of speciesID
                return region;
        }


        return null;
    }
}