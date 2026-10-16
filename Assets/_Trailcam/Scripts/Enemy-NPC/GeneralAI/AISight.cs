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
}

