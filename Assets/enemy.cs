using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class enemy : MonoBehaviour
{
    [Header("Patrol Settings")]
    public Transform waypointA;
    public Transform waypointB;
    public float speed = 2f;

    private Transform currentTarget;
    private Vector3 movementDirection;

    [Header("Vision Settings")]
    public Transform eyeOrigin;
    public float viewRadius = 6f;
    public float viewAngle = 60f;
    public LayerMask targetMask;
    public LayerMask obstructionMask;

    [Header("Vision Ray Visual")]
    public Color rayColor = new Color(1f, 0f, 0f, 0.5f);
    private LineRenderer lineRenderer;

    public bool isSleeping = false;

    void Start()
    {
        currentTarget = waypointB;
        EnemySleepController.Instance?.RegisterEnemy(this);
        UpdateMovementDirection();
        SetupLineRenderer();
    }

    void Update()
    {
        if (isSleeping) return;

        Patrol();
        DetectPlayer();
        DrawVisionRay();
    }
    public void SetSleeping(bool sleep)
    {
        isSleeping = sleep;
    }

    void Patrol()
    {
        transform.position = Vector3.MoveTowards(transform.position, currentTarget.position, speed * Time.deltaTime);

        if (Vector3.Distance(transform.position, currentTarget.position) < 0.1f)
        {
            currentTarget = (currentTarget == waypointA) ? waypointB : waypointA;
            FlipVisual();
            UpdateMovementDirection();
        }
    }

    void FlipVisual()
    {
        Vector3 scale = transform.localScale;
        scale.z *= -1;
        transform.localScale = scale;
    }

    void UpdateMovementDirection()
    {
        movementDirection = (currentTarget.position - transform.position).normalized;
    }

    void DetectPlayer()
    {
        Collider[] targets = Physics.OverlapSphere(eyeOrigin.position, viewRadius, targetMask);

        foreach (var target in targets)
        {
            Transform targetTransform = target.transform;
            Vector3 dirToTarget = (targetTransform.position - eyeOrigin.position).normalized;

            if (Vector3.Angle(movementDirection, dirToTarget) < viewAngle / 2f)
            {
                float distance = Vector3.Distance(eyeOrigin.position, targetTransform.position);

                if (!Physics.Raycast(eyeOrigin.position, dirToTarget, distance, obstructionMask))
                {
                    Debug.Log("Player spotted and killed!");
                    ThirdPersonController playerController = targetTransform.GetComponent<ThirdPersonController>();
                    if (playerController != null) playerController.HandleDeath();
                }
            }
        }
    }

    void SetupLineRenderer()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.useWorldSpace = true;
        lineRenderer.loop = false;
        lineRenderer.startWidth = 0.05f;
        lineRenderer.endWidth = 0.05f;
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.startColor = rayColor;
        lineRenderer.endColor = rayColor;
    }

    void DrawVisionRay()
    {
        if (eyeOrigin == null || lineRenderer == null) return;

        Vector3 origin = eyeOrigin.position;
        Vector3 baseDir = new Vector3(movementDirection.x, 0f, movementDirection.z).normalized;

        Vector3 leftDir = Quaternion.Euler(0, -viewAngle / 2f, 0) * baseDir;
        Vector3 rightDir = Quaternion.Euler(0, viewAngle / 2f, 0) * baseDir;

        Vector3 leftEnd = origin + leftDir * viewRadius;
        Vector3 rightEnd = origin + rightDir * viewRadius;

        leftEnd.y = origin.y;
        rightEnd.y = origin.y;

        lineRenderer.positionCount = 4;
        lineRenderer.SetPosition(0, origin);
        lineRenderer.SetPosition(1, leftEnd);
        lineRenderer.SetPosition(2, rightEnd);
        lineRenderer.SetPosition(3, origin);

        Ray ray = new Ray(origin, baseDir);
        if (Physics.Raycast(ray, out RaycastHit hit, viewRadius, obstructionMask))
        {
            lineRenderer.enabled = false;
        }
        else
        {
            lineRenderer.enabled = true;
        }
    }

    void OnDrawGizmosSelected()
    {
        if (eyeOrigin == null || waypointA == null || waypointB == null) return;

        Gizmos.color = new Color(1f, 0f, 0f, 0.2f);
        Vector3 leftDir = Quaternion.Euler(0, -viewAngle / 2f, 0) * movementDirection;
        Vector3 rightDir = Quaternion.Euler(0, viewAngle / 2f, 0) * movementDirection;
        Gizmos.DrawLine(eyeOrigin.position, eyeOrigin.position + leftDir * viewRadius);
        Gizmos.DrawLine(eyeOrigin.position, eyeOrigin.position + rightDir * viewRadius);
        Gizmos.DrawLine(eyeOrigin.position + leftDir * viewRadius, eyeOrigin.position + rightDir * viewRadius);
    }
}
