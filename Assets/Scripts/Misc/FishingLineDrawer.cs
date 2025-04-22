using UnityEngine;

public class FishingLineDrawer : MonoBehaviour
{
    public Transform playerTransform;
    public Transform lureTransform;
    private LineRenderer lineRenderer;

    private void OnEnable()
    {
        Lure.OnLureCreated += SetLureTarget;
        Lure.OnLureDestroyed += ClearLureTarget;
    }

    private void OnDisable()
    {
        Lure.OnLureCreated -= SetLureTarget;
        Lure.OnLureDestroyed -= ClearLureTarget;
    }

    private void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        // Hide line initially
        if (lineRenderer != null && lureTransform == null)
        {
            lineRenderer.enabled = false;
        }
    }

    private void Update()
    {
        if (lureTransform != null && lineRenderer != null)
        {
            // Show line when lure exists
            if (!lineRenderer.enabled)
            {
                lineRenderer.enabled = true;
            }

            // Update line positions
            lineRenderer.SetPosition(0, playerTransform.position);
            lineRenderer.SetPosition(1, lureTransform.position);
        }
    }

    void SetLureTarget(Transform lure)
    {
        lureTransform = lure;
    }
    
    void ClearLureTarget()
    {
        lureTransform = null;
        
        // Hide the line renderer when lure is destroyed
        if (lineRenderer != null)
        {
            lineRenderer.enabled = false;
        }
    }
}
