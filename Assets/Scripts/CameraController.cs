using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform target; // The lure to follow
    public float smoothSpeed = 0.125f;
    public Vector3 offset = new Vector3(0, 5, -10); // Z is for camera depth only

    // Boundary settings
    public float minX = -53f;
    public float maxX = 20f;
    public float minY = -7f;
    public float maxY = 27f;

    // Movement control
    public bool enableBoundaries = true;
    public bool allowYMovement = true; // Allow Y movement even when X is locked

    private void LateUpdate()
    {
        if (target == null)
            return;

        // Calculate desired position (original follow behavior)
        Vector3 desiredPosition = target.position + offset;

        // Apply smooth movement
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);

        // Apply boundary restrictions if enabled
        if (enableBoundaries)
        {
            // Apply X boundary
            if (smoothedPosition.x < minX)
                smoothedPosition.x = minX;
            else if (smoothedPosition.x > maxX)
                smoothedPosition.x = maxX;

            // Apply Y boundary
            if (smoothedPosition.y < minY)
                smoothedPosition.y = minY;
            else if (smoothedPosition.y > maxY)
                smoothedPosition.y = maxY;
        }

        // Keep Z the same as intended in offset (for camera depth)
        smoothedPosition.z = desiredPosition.z;

        // Apply the final position, ensuring Y movement continues regardless of X boundaries
        if (allowYMovement && enableBoundaries)
        {
            // If X hit boundaries but still want Y movement, use original Y calculation
            // Check if we're at a boundary
            bool hitXBoundary = (smoothedPosition.x == minX || smoothedPosition.x == maxX);

            if (hitXBoundary)
            {
                // If we hit X boundary, ensure Y movement continues
                transform.position = new Vector3(
                    smoothedPosition.x,
                    desiredPosition.y, // Use desired Y position directly
                    smoothedPosition.z
                );
            }
            else
            {
                // No boundaries hit, use normal smoothed position
                transform.position = smoothedPosition;
            }
        }
        else
        {
            // Otherwise just use the full smoothed position
            transform.position = smoothedPosition;
        }
    }

    // Visual debugging in editor
    private void OnDrawGizmos()
    {
        if (!enableBoundaries)
            return;

        Gizmos.color = Color.red;

        // Draw the 2D boundary box
        Vector3 bottomLeft = new Vector3(minX, minY, transform.position.z);
        Vector3 bottomRight = new Vector3(maxX, minY, transform.position.z);
        Vector3 topLeft = new Vector3(minX, maxY, transform.position.z);
        Vector3 topRight = new Vector3(maxX, maxY, transform.position.z);

        Gizmos.DrawLine(bottomLeft, bottomRight);
        Gizmos.DrawLine(bottomRight, topRight);
        Gizmos.DrawLine(topRight, topLeft);
        Gizmos.DrawLine(topLeft, bottomLeft);
    }
}