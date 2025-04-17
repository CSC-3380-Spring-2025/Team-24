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
    public string speciesID;

    [Header("Fish Behavior Bounds")]
    public float maxDistanceFromCenter = 5f;

    [Header("Visualization")]
    public Color waterSurfaceColor = Color.blue; //Color for water surface in edit
    public Color minDepthColor = Color.red; //Color for min depth in edit
    public Color maxDepthColor = Color.red; //Color for max depth in edit
    public bool showDepthLevel = true;
    public bool showFishBounds = true;

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
    
    private void OnDrawGizmos()
    {
        if (showDepthLevel)
        {
            float regionWidth = size.x;
            Vector2 leftPoint = transform.position - new Vector3(regionWidth / 2, 0,0);
            Vector2 rightPoint = transform.position + new Vector3(regionWidth / 2, 0,0);

            //Draw min depth
            Gizmos.color = minDepthColor;
            float minDepthY = GetYPositionForDepth(minDepth);
            Gizmos.DrawLine(
                new Vector3(leftPoint.x, minDepthY, 0),
                new Vector3(rightPoint.x, minDepthY, 0)
                );

            Gizmos.color = maxDepthColor;
            float maxDepthY = GetYPositionForDepth(maxDepth);
            Gizmos.DrawLine(
                new Vector3(leftPoint.x, maxDepthY, 0),
                new Vector3(rightPoint.x, maxDepthY, 0)
                );
        }

    }    
}
