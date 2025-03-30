using UnityEngine;

[ExecuteAlways] //Allows code to execute in edit mode!
public class SpawnRegion : MonoBehaviour
{
    [Header("Region Dimensions")]
    public Vector2 size = new Vector2(10f, 5f);
    public float waterSurfaceY = 16f;
    public float minDepth = 0f;
    public float maxDepth = 10f;

    [Header("Region Properties")]
    public string regionID;

    [Header("Visualization")]
    public Color gizmoColor = Color.green; //Color for gizmo in edit

    public Bounds GetBounds()
    {
        return new Bounds(transform.position, size);
    }

    //Get the actual Y cord for a specific depth from the water surface
    public float GetYPositionForDepth(float depthFromSurface)
    {
        return waterSurfaceY - depthFromSurface;
    }

    public Vector2 GetRandomPosition()
    {
        Bounds bounds = GetBounds(); //Get the bounds of the spawn region
        
        return new Vector2(
            Random.Range(bounds.min.x, bounds.max.x), //Random X within the bounds
            Random.Range(GetYPositionForDepth(maxDepth), GetYPositionForDepth(minDepth)) //Random Y within the depth range
        );
    }

    public string speciesID; //SpeciesID
    
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = gizmoColor;
        Gizmos.DrawWireCube(transform.position, size); //Draws wire cube, well square bc we passing 2d vector 
    }    
}
