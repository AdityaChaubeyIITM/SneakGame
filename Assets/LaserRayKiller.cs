using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class LaserRayKiller : MonoBehaviour
{
    [Tooltip("Start point of the laser ray (Empty GameObject)")]
    public Transform pointA;

    [Tooltip("End point of the laser ray (Empty GameObject)")]
    public Transform pointB;

    [Tooltip("Which layers the ray should detect (e.g., Player layer)")]
    public LayerMask detectionLayer;

    [Tooltip("Show the debug ray in Scene view")]
    public bool drawDebugRay = true;

    private LineRenderer lineRenderer;

    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();

        // Configure LineRenderer
        lineRenderer.positionCount = 2;
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.startColor = Color.red;
        lineRenderer.endColor = Color.red;
        lineRenderer.startWidth = 0.05f;
        lineRenderer.endWidth = 0.05f;
        lineRenderer.useWorldSpace = true;
    }

    void Update()
    {
        if (pointA == null || pointB == null) return;

        Vector3 start = pointA.position;
        Vector3 end = pointB.position;
        Vector3 direction = end - start;
        float distance = direction.magnitude;

        // Draw laser line
        lineRenderer.SetPosition(0, start);
        lineRenderer.SetPosition(1, end);

        // Optional debug ray in Scene view
        if (drawDebugRay)
        {
            Debug.DrawLine(start, end, Color.red);
        }

        // Check if player is in between
        if (Physics.Raycast(start, direction.normalized, out RaycastHit hit, distance, detectionLayer))
        {
            ThirdPersonController player = hit.collider.GetComponent<ThirdPersonController>();
            if (player != null)
            {
                player.HandleDeath();
            }
        }
    }
}
