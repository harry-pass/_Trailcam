using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Watches its AISight cone and raises OnMotionDetected when anything on the
/// detectable layers moves while visible to the camera.
/// </summary>
[RequireComponent(typeof(AISight))]
public class Trailcam : MonoBehaviour
{
    public static event Action<Trailcam> OnMotionDetected;

    [Header("Trailcam Parameters")]
    [SerializeField] string cameraName = "Trailcam";
    [SerializeField] LayerMask detectableMask = ~0;
    [SerializeField] float scanInterval = 0.2f;
    [SerializeField] float motionThreshold = 0.05f;   // metres moved between scans to count as motion
    [SerializeField] float notificationCooldown = 5f;

    [Header("Components")]
    [SerializeField] AISight sight;

    public string CameraName => cameraName;

    readonly Dictionary<Transform, Vector3> lastPositions = new Dictionary<Transform, Vector3>();
    readonly Dictionary<Transform, Vector3> currentPositions = new Dictionary<Transform, Vector3>();
    readonly Collider[] hits = new Collider[32];
    float nextScanTime;
    float lastNotificationTime = float.NegativeInfinity;

    void OnValidate()
    {
        if (sight == null)
        {
            sight = GetComponent<AISight>();
        }
    }

    void Update()
    {
        if (Time.time < nextScanTime) return;
        nextScanTime = Time.time + scanInterval;
        Scan();
    }

    void Scan()
    {
        currentPositions.Clear();
        bool motionSeen = false;

        int count = Physics.OverlapSphereNonAlloc(transform.position, sight.Range, hits, detectableMask, QueryTriggerInteraction.Ignore);
        for (int i = 0; i < count; i++)
        {
            // Use the rigidbody root when there is one so multi-collider objects count once.
            Transform target = hits[i].attachedRigidbody != null ? hits[i].attachedRigidbody.transform : hits[i].transform;
            if (target.IsChildOf(transform) || currentPositions.ContainsKey(target)) continue;

            Vector3 position = target.position;
            currentPositions.Add(target, position);

            if (motionSeen) continue;
            if (!lastPositions.TryGetValue(target, out Vector3 previous)) continue;
            if ((position - previous).sqrMagnitude < motionThreshold * motionThreshold) continue;
            if (!sight.CanSeeTarget(target, out _)) continue;

            motionSeen = true;
        }

        lastPositions.Clear();
        foreach (var pair in currentPositions)
        {
            lastPositions.Add(pair.Key, pair.Value);
        }

        if (motionSeen && Time.time - lastNotificationTime >= notificationCooldown)
        {
            lastNotificationTime = Time.time;
            OnMotionDetected?.Invoke(this);
        }
    }
}
