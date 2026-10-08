using UnityEngine;

public class AIPerception : MonoBehaviour
{
    public enum KnowledgeSource { None, Sound, Sight }

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
    public KnowledgeSource LastKnowledgeSource { get; private set; } = KnowledgeSource.None;
    public float TimeSinceKnown => Time.time - lastKnownTime;

    void OnValidate()
    {
        if (sight == null) sight = GetComponent<AISight>();
        if (hearing == null) hearing = GetComponent<AIHearing>();
    }

    void Awake()
    {
        TryFindTarget();
    }

    void Update()
    {
        // Retry each frame in case the player spawns after the enemy.
        if (target == null)
        {
            TryFindTarget();
            if (target == null) return;
        }
        PerceptionUpdate();
    }

    void TryFindTarget()
    {
        GameObject targetObject = GameObject.FindGameObjectWithTag(TargetTag);
        if (targetObject != null) target = targetObject.transform;
    }

    void PerceptionUpdate()
    {
        // Always drain the hearing buffer so stale sounds can't resurface after sight is lost.
        bool heard = hearing.TryConsumeHeardSound(out Vector3 heardPosition);
        CanSeeTarget = sight.CanSeeTarget(target, out Vector3 seenPosition);

        if (CanSeeTarget)
        {
            Remember(seenPosition, KnowledgeSource.Sight);
        }
        else if (heard)
        {
            Remember(heardPosition, KnowledgeSource.Sound);
        }

        HasKnownTargetPosition = Time.time - lastKnownTime <= MemoryDuration;
    }

    void Remember(Vector3 position, KnowledgeSource source)
    {
        LastKnownTargetPosition = position;
        LastKnowledgeSource = source;
        lastKnownTime = Time.time;
    }

    /// <summary>Manually clear memory of the target, e.g. after a search concludes.</summary>
    public void ForgetTarget()
    {
        HasKnownTargetPosition = false;
        LastKnowledgeSource = KnowledgeSource.None;
        lastKnownTime = float.NegativeInfinity;
    }
}