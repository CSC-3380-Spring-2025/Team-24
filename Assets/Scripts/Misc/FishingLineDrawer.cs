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
        lineRenderer.enabled = false;
    }

    private void Update()
    {
        if (lureTransform != null)
        {
            lineRenderer.SetPosition(0, playerTransform.position);
            lineRenderer.SetPosition(1, lureTransform.position);
        }
    }

    void SetLureTarget(Transform lure)
    {
        lureTransform = lure;
        lineRenderer.enabled = true;
    }

    void ClearLureTarget()
    {
        lureTransform = null;
        lineRenderer.enabled = false;
    }

}
