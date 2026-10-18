using UnityEngine;

/// <summary>
/// Field-of-view + line-of-sight sensor. Call CanSeeTarget() to check whether
/// a given transform is currently visible from this sensor's eyes.
/// </summary>
public class AISight : MonoBehaviour
{
    [Header("AI Sight Parameters")]
    [SerializeField] float DetectionRange = 15f;
    [SerializeField, Range(0f, 360f)] float FieldOfViewAngle = 120f;
    [SerializeField] LayerMask ObstructionMask;

    [Header("Components")]
    [SerializeField] Transform eyes;

    void OnValidate()
    {
        if (eyes == null)
        {
            eyes = transform;
        }
    }

    [SerializeField] float TargetHeightOffset = 1.2f;   // aim at chest; set to 0 if your player's pivot is already centered

    public string LastFailReason { get; private set; }

    public bool CanSeeTarget(Transform target, out Vector3 lastSeenPosition)
    {
        lastSeenPosition = default;

        Vector3 targetPoint = target.position + Vector3.up * TargetHeightOffset;
        Vector3 toTarget = targetPoint - eyes.position;
        float distance = toTarget.magnitude;
        if (distance > DetectionRange) { LastFailReason = "range"; return false; }

        // Horizontal angle only. A full 3D angle makes a tall bear blind to anything close to it.
        Vector3 flatForward = Vector3.ProjectOnPlane(eyes.forward, Vector3.up);
        Vector3 flatToTarget = Vector3.ProjectOnPlane(toTarget, Vector3.up);
        if (flatForward.sqrMagnitude > 0.0001f && flatToTarget.sqrMagnitude > 0.0001f
            && Vector3.Angle(flatForward, flatToTarget) > FieldOfViewAngle * 0.5f)
        {
            LastFailReason = "fov";
            return false;
        }

        if (Physics.Raycast(eyes.position, toTarget.normalized, distance, ObstructionMask))
        {
            LastFailReason = "blocked";
            return false;
        }

        LastFailReason = "visible";
        lastSeenPosition = target.position;   // navigate to the ground position, not the chest
        return true;
    }

    // Debug view cone: green while the target is visible, yellow otherwise.
    // Shows in the Scene view, and in the Game view when its Gizmos toggle is on.
    void OnDrawGizmos()
    {
        Transform origin = eyes != null ? eyes : transform;
        Vector3 forward = Vector3.ProjectOnPlane(origin.forward, Vector3.up);
        if (forward.sqrMagnitude < 0.0001f) return;
        forward.Normalize();

        Gizmos.color = LastFailReason == "visible" ? Color.green : Color.yellow;
        Vector3 pos = origin.position;
        float half = FieldOfViewAngle * 0.5f;
        const int segments = 16;

        Vector3 prev = pos + Quaternion.AngleAxis(-half, Vector3.up) * forward * DetectionRange;
        Gizmos.DrawLine(pos, prev);
        for (int i = 1; i <= segments; i++)
        {
            float angle = Mathf.Lerp(-half, half, i / (float)segments);
            Vector3 next = pos + Quaternion.AngleAxis(angle, Vector3.up) * forward * DetectionRange;
            Gizmos.DrawLine(prev, next);
            prev = next;
        }
        Gizmos.DrawLine(pos, prev);

        Gizmos.color = Color.red;
        Gizmos.DrawLine(pos, pos + forward * DetectionRange);   // exact look direction
    }
}

