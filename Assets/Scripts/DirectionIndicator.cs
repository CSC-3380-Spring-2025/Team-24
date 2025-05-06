using System;
using Unity.VisualScripting;
using UnityEngine;

public class DirectionIndicator : MonoBehaviour
{
    [Header("Line Renderer Settings")]
    public LineRenderer lineRenderer;
    public float lineLength = 5f;
    public int lineSegmentCount = 2;
    public float width = 2f;

    [Header("Angle Settings")]
    [Range(0, 90)]
    public float angle = 45f;
    public float angleChangeSpeed = 30f;


    [Header("Player Reference")]
    public Transform playerTransform;

    [Header("KeyBindings")]
    public KeyCode increaseAnglekey = KeyCode.UpArrow;
    public KeyCode decreaseAngleKey = KeyCode.DownArrow;
    public KeyCode castKey = KeyCode.Space;

    private Vector2 currentDirection;

    
    private void Start()
    {
        if (lineRenderer == null)
        {
            lineRenderer = gameObject.AddComponent<LineRenderer>();
        }
        lineRenderer.positionCount = lineSegmentCount;
        lineRenderer.widthMultiplier = width;
        UpdateDirection();
        UpdateLine();
    }

    private void Update()
    {
        if (Input.GetKey(increaseAnglekey))
        {
            angle += angleChangeSpeed * Time.deltaTime;
        }
        else if (Input.GetKey(decreaseAngleKey))
        {
            angle -= angleChangeSpeed * Time.deltaTime;
        }

        angle = Mathf.Clamp(angle, 0f, 90f);

        UpdateDirection();
        UpdateLine();
    }

    void UpdateDirection()
    {
        //Creates unit vector in direction angle
        currentDirection = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));
    }

    void UpdateLine()
    {
        Vector2 origin = playerTransform.position;
        
        //Currently lineSegmentCount only needs to be 2 for a straigt line however 
        //We can simulate parabolic motion with a simulated magnitude in future.
        for (int i = 0; i < lineSegmentCount; i++)
        {
            float t = (float)i / (lineSegmentCount - 1); // normalized position along the line
            Vector2 point = origin + (Vector2)(currentDirection * lineLength * t);
            lineRenderer.SetPosition(i, point);
        }
    }

    public Vector2 getCurrentDirection()
    {
        return this.currentDirection;
    }


}
