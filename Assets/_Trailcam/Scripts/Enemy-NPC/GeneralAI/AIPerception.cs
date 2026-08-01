using UnityEngine;

public class AIPerception : MonoBehaviour
{
    [Header("Perception Parameters")]
    [SerializeField] float MemoryDuration = 6f; // how long the AI keeps "knowing" after losing the target
    [SerializeField] string TargetTag = "Player";

    [Header("Components")]
    [SerializeField] AISight sight;
    [SerializeField] AIHearing hearing;

    Transform target;
    float lastKnownTime = float.NegativeInfinity;

    public bool CanSeeTarget { get; private set; }
    public bool HasKnownTargetPosition { get; private set; }
    public Vector3 LastKnownTargetPosition { get; private set; }

    void OnValidate()
    {
        if (sight == null) sight = GetComponent<AISight>();
        if (hearing == null) hearing = GetComponent<AIHearing>();
    }

    void Awake()
    {
        GameObject targetObject = GameObject.FindGameObjectWithTag(TargetTag);
        if (targetObject != null) target = targetObject.transform;
    }

    void Update()
    {
        if (target == null) return;
        PerceptionUpdate();
    }

    void PerceptionUpdate()
    {
        CanSeeTarget = sight.CanSeeTarget(target, out Vector3 seenPosition);

        if (CanSeeTarget)
        {
            LastKnownTargetPosition = seenPosition;
            lastKnownTime = Time.time;
        }
        else if (hearing.TryConsumeHeardSound(out Vector3 heardPosition))
        {
            LastKnownTargetPosition = heardPosition;
            lastKnownTime = Time.time;
        }

        HasKnownTargetPosition = Time.time - lastKnownTime <= MemoryDuration;
    }

    /// <summary>Manually clear memory of the target, e.g. after a search concludes.</summary>
    public void ForgetTarget()
    {
        HasKnownTargetPosition = false;
        lastKnownTime = float.NegativeInfinity;
    }
}

