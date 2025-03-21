using UnityEngine;

[CreateAssetMenu(fileName = "NewFishType", menuName = "Fish/FishType")]
public class FishType : ScriptableObject
{
    public string typeName;
    public GameObject prefab;
    public string speciesID;
    public float spawnWeight; //rarity of fish
    public SpawnRegion spawnRegion;
    public float spawnDepth;
    
    public int schoolSizeMin = 3;
    public int schoolSizeMax = 10;
    
    //AI paremeters 
    public float maxSpeed = 2f;
    public float maxForce = 0.5f;
    public float neighborRadius = 3f;
    public float separationDistance = 1f;
    public float alignmentWeight = 1f;
    public float cohesionWeight = 1f;
    public float separationWeight = 1.5f;
    public float wanderWeight = 0.5f;
    public float homeAttractionWeight = 1f;
}
