using UnityEngine;

public class FishingLineDrawer : MonoBehaviour
{
    public Transform playerTransform;
    public Transform lureTransform;
    private LineRenderer lineRenderer;

    private void OnEnable()
    {
        Lure.OnLureCreated += SetLureTarget;
    }

    private void OnDisable()
    {
        Lure.OnLureCreated -= SetLureTarget;
    }

    private void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
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
    }
}
