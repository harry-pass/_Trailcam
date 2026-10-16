using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Turn To Face Last Known", story: "[Agent] turns to face the last known target position", category: "Action/Perception", id: "a1a28e4c76a4cf2628bb1fbc42a38967")]
public partial class TurnToFaceLastKnownAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Agent;
    [SerializeReference] public BlackboardVariable<float> TurnSpeed = new BlackboardVariable<float>(180f);   // degrees per second
    [SerializeReference] public BlackboardVariable<float> Tolerance = new BlackboardVariable<float>(8f);     // degrees
    [SerializeReference] public BlackboardVariable<float> Timeout = new BlackboardVariable<float>(3f);

    EnemyAgent enemy;
    Vector3 facePoint;
    float startTime;

    protected override Status OnStart()
    {
        enemy = Agent.Value.GetComponent<EnemyAgent>();
        if (enemy == null || !enemy.Perception.HasKnownTargetPosition) return Status.Failure;

        facePoint = enemy.Perception.LastKnownTargetPosition;
        enemy.Movement.Stop();                // stand still while turning
        startTime = Time.time;
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        Transform t = Agent.Value.transform;
        Vector3 flat = Vector3.ProjectOnPlane(facePoint - t.position, Vector3.up);
        if (flat.sqrMagnitude < 0.01f) return Status.Success;

        Quaternion goal = Quaternion.LookRotation(flat.normalized, Vector3.up);
        t.rotation = Quaternion.RotateTowards(t.rotation, goal, TurnSpeed.Value * Time.deltaTime);

        bool facing = Quaternion.Angle(t.rotation, goal) <= Tolerance.Value;
        return (facing || Time.time - startTime > Timeout.Value) ? Status.Success : Status.Running;
    }
}

