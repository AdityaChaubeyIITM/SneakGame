using UnityEngine;

public class EnemyVisionRaycast : MonoBehaviour
{
    public float viewRadius = 10f;
    public float viewAngle = 90f;
    public LayerMask targetMask;       // Layer of the player
    public LayerMask obstructionMask;  // Layer for walls/obstacles

    public Transform eyeOrigin; // Where rays cast from

    void Update()
    {
        DetectPlayer();
    }

    void DetectPlayer()
    {
        Collider[] rangeChecks = Physics.OverlapSphere(transform.position, viewRadius, targetMask);

        foreach (var target in rangeChecks)
        {
            Transform targetTransform = target.transform;
            Vector3 dirToTarget = (targetTransform.position - eyeOrigin.position).normalized;

            if (Vector3.Angle(transform.forward, dirToTarget) < viewAngle / 2f)
            {
                float distToTarget = Vector3.Distance(eyeOrigin.position, targetTransform.position);

                if (!Physics.Raycast(eyeOrigin.position, dirToTarget, distToTarget, obstructionMask))
                {
                    Debug.Log("Player spotted via raycast!");
                    //PlayerDeathHandler death = targetTransform.GetComponent<PlayerDeathHandler>();
                    //"if (death != null) death.Die();"
                }
            }
        }
    }

    // Optional: Draw vision cone in scene view
    void OnDrawGizmosSelected()
    {
        if (eyeOrigin == null) return;

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, viewRadius);

        Vector3 leftBoundary = DirFromAngle(-viewAngle / 2, false);
        Vector3 rightBoundary = DirFromAngle(viewAngle / 2, false);

        Gizmos.color = Color.red;
        Gizmos.DrawLine(eyeOrigin.position, eyeOrigin.position + leftBoundary * viewRadius);
        Gizmos.DrawLine(eyeOrigin.position, eyeOrigin.position + rightBoundary * viewRadius);
    }

    Vector3 DirFromAngle(float angleInDegrees, bool angleIsGlobal)
    {
        if (!angleIsGlobal)
        {
            angleInDegrees += transform.eulerAngles.y;
        }
        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }
}
