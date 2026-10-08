using System;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine;
using Action = Unity.Behavior.Action; // avoids clash with System.Action

[Serializable, GeneratePropertyBag]
[Condition(name: "Can See Target", story: "[Agent] can see the target", category: "Conditions", id: "6f1c0b7e2a9d4c3f8e5b1a7d9c2e4f60")]
public partial class CanSeeTargetCondition : Condition
{
    [SerializeReference] public BlackboardVariable<GameObject> Agent;
    EnemyAgent enemy;

    public override bool IsTrue()
    {
        if (enemy == null) enemy = Agent.Value.GetComponent<EnemyAgent>();
        return enemy != null && enemy.Perception.CanSeeTarget;
    }
}

[Serializable, GeneratePropertyBag]
[Condition(name: "Has Known Target Position", story: "[Agent] has a known target position", category: "Conditions", id: "b3d82e41c7f94a1d90e6a5c2f8b7d103")]
public partial class HasKnownTargetCondition : Condition
{
    [SerializeReference] public BlackboardVariable<GameObject> Agent;
    EnemyAgent enemy;

    public override bool IsTrue()
    {
        if (enemy == null) enemy = Agent.Value.GetComponent<EnemyAgent>();
        return enemy != null && enemy.Perception.HasKnownTargetPosition;
    }
}

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Get Last Known Target", story: "[Agent] stores last known position in [Position], run: [Run]", category: "Action/Perception", id: "0e7a4c95d1b24f3a8c6e2d9b5f1a7c48")]
public partial class GetLastKnownTargetAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Agent;
    [SerializeReference] public BlackboardVariable<Vector3> Position;
    [SerializeReference] public BlackboardVariable<bool> Run;

    protected override Status OnStart()
    {
        var enemy = Agent.Value.GetComponent<EnemyAgent>();
        if (enemy == null || !enemy.Perception.HasKnownTargetPosition) return Status.Failure;

        Position.Value = enemy.Perception.LastKnownTargetPosition;
        // Saw it = hunt it. Only heard it = creep over cautiously.
        Run.Value = enemy.Perception.LastKnowledgeSource == AIPerception.KnowledgeSource.Sight;
        return Status.Success;
    }
}

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Move To Position", story: "[Agent] moves to [Position], run: [Run]", category: "Action/Navigation", id: "d94f1a60b8e34c27a5d3e7f2c1b09e85")]
public partial class MoveToPositionAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Agent;
    [SerializeReference] public BlackboardVariable<Vector3> Position;
    [SerializeReference] public BlackboardVariable<bool> Run;

    AIMovement movement;

    protected override Status OnStart()
    {
        movement = Agent.Value.GetComponent<AIMovement>();
        if (movement == null) return Status.Failure;
        return movement.TryMoveTo(Position.Value, Run.Value) ? Status.Running : Status.Failure;
    }

    protected override Status OnUpdate()
    {
        if (movement.PathFailed) return Status.Failure;
        return movement.HasArrived ? Status.Success : Status.Running;
    }

    protected override void OnEnd()
    {
        // If we were interrupted mid-walk, don't keep following a stale path.
        if (movement != null && !movement.HasArrived) movement.Stop();
    }
}

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Look Around", story: "[Agent] looks around for [Duration] seconds", category: "Action/Navigation", id: "5a2c8e17f3d64b90a1e4c7d2b6f93a05")]
public partial class LookAroundAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Agent;
    [SerializeReference] public BlackboardVariable<float> Duration = new BlackboardVariable<float>(3f);
    [SerializeReference] public BlackboardVariable<float> SweepAngle = new BlackboardVariable<float>(120f);

    float elapsed;
    float startYaw;

    protected override Status OnStart()
    {
        var movement = Agent.Value.GetComponent<AIMovement>();
        if (movement != null) movement.Stop();
        elapsed = 0f;
        startYaw = Agent.Value.transform.eulerAngles.y;
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        elapsed += Time.deltaTime;
        float yaw = startYaw + Mathf.Sin(elapsed / Duration.Value * Mathf.PI * 2f) * SweepAngle.Value * 0.5f;
        Agent.Value.transform.rotation = Quaternion.Euler(0f, yaw, 0f);
        return elapsed >= Duration.Value ? Status.Success : Status.Running;
    }
}

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Chase Target", story: "[Agent] chases target until within [AttackRange]", category: "Action/Combat", id: "c18b7d40e2a94f65b3d9a1e8f4c26b70")]
public partial class ChaseTargetAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Agent;
    [SerializeReference] public BlackboardVariable<float> AttackRange = new BlackboardVariable<float>(1.8f);
    [SerializeReference] public BlackboardVariable<float> RepathInterval = new BlackboardVariable<float>(0.2f);

    EnemyAgent enemy;
    float nextRepath;

    protected override Status OnStart()
    {
        enemy = Agent.Value.GetComponent<EnemyAgent>();
        nextRepath = 0f;
        return enemy != null ? Status.Running : Status.Failure;
    }

    protected override Status OnUpdate()
    {
        var p = enemy.Perception;

        // Lost sight: fail so the selector falls through to Investigate.
        if (!p.CanSeeTarget) return Status.Failure;

        if (Time.time >= nextRepath)
        {
            enemy.Movement.TryMoveTo(p.LastKnownTargetPosition, true);
            nextRepath = Time.time + RepathInterval.Value;
        }

        float dist = Vector3.Distance(enemy.transform.position, p.LastKnownTargetPosition);
        return dist <= AttackRange.Value ? Status.Success : Status.Running;
    }
}

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Forget Target", story: "[Agent] gives up on the target", category: "Action/Perception", id: "7e4d2b91a5c84f30b6e1d8a3c9f52e17")]
public partial class ForgetTargetAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Agent;

    protected override Status OnStart()
    {
        var enemy = Agent.Value.GetComponent<EnemyAgent>();
        if (enemy != null) enemy.Perception.ForgetTarget();
        return Status.Success;
    }
}
