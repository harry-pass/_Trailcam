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

    public bool CanSeeTarget(Transform target, out Vector3 lastSeenPosition)
    {
        lastSeenPosition = default;

        Vector3 toTarget = target.position - eyes.position;
        float distance = toTarget.magnitude;
        if (distance > DetectionRange) return false;

        float angle = Vector3.Angle(eyes.forward, toTarget);
        if (angle > FieldOfViewAngle * 0.5f) return false;

        // ObstructionMask should only contain walls/geometry, not the target's own layer,
        // so any hit here means the view is blocked.
        if (Physics.Raycast(eyes.position, toTarget.normalized, distance, ObstructionMask))
        {
            return false;
        }

        lastSeenPosition = target.position;
        return true;
    }
}

