using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class AIMovement : MonoBehaviour
{
    [Header("AI Movement Parameters")]
    [SerializeField] float WalkSpeed = 2f;
    [SerializeField] float RunSpeed = 5.5f;
    [SerializeField] float StoppingDistance = 0.3f;

    [Header("Components")]
    [SerializeField] NavMeshAgent agent;

    public bool HasArrived => !agent.pathPending && agent.remainingDistance <= agent.stoppingDistance
                               && (!agent.hasPath || agent.velocity.sqrMagnitude < 0.01f);

    void OnValidate()
    {
        if (agent == null) agent = GetComponent<NavMeshAgent>();
        agent.stoppingDistance = StoppingDistance;
    }

    void Awake()
    {
        agent.stoppingDistance = StoppingDistance;
    }

    public void MoveTo(Vector3 destination, bool run)
    {
        agent.speed = run ? RunSpeed : WalkSpeed;
        agent.isStopped = false;
        agent.SetDestination(destination);
    }

    /// <summary>Snaps the destination onto the NavMesh first. Returns false if no valid point was found nearby.</summary>
    public bool TryMoveTo(Vector3 destination, bool run, float sampleRadius = 2f)
    {
        if (NavMesh.SamplePosition(destination, out NavMeshHit hit, sampleRadius, NavMesh.AllAreas))
        {
            MoveTo(hit.position, run);
            return true;
        }
        return false;
    }

    public void Stop()
    {
        if (agent.isOnNavMesh)
        {
            agent.isStopped = true;
            agent.ResetPath();
        }
    }
}

