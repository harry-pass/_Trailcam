using System;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Random = UnityEngine.Random;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "RandomRoam", story: "[Agent] Roams randomly in territory, between major POIs", category: "Action", id: "0255fefd656a1acb52f9167f17302b76")]
public partial class RandomRoamAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Agent;
    [SerializeReference] public BlackboardVariable<float> PoiChance = new BlackboardVariable<float>(0.7f);       // chance to head for a POI vs a random spot
    [SerializeReference] public BlackboardVariable<float> WanderPauseMin = new BlackboardVariable<float>(2f);    // pause at random spots
    [SerializeReference] public BlackboardVariable<float> WanderPauseMax = new BlackboardVariable<float>(8f);
    [SerializeReference] public BlackboardVariable<float> MaxTravelTime = new BlackboardVariable<float>(90f);    // give up if stuck

    AIMovement movement;
    BearTerritory territory;
    TerritoryPOI currentPoi;
    bool dwelling;
    float travelStartTime;
    float dwellDuration;
    float dwellEndTime;

    protected override Status OnStart()
    {
        movement = Agent.Value.GetComponent<AIMovement>();
        if (territory == null) territory = UnityEngine.Object.FindFirstObjectByType<BearTerritory>();
        if (movement == null || territory == null) return Status.Failure;

        dwelling = false;
        currentPoi = null;

        Vector3 destination = default;
        bool hasDestination = false;

        if (Random.value < PoiChance.Value)
        {
            currentPoi = territory.PickPOI();
            if (currentPoi != null)
            {
                destination = currentPoi.GetArrivalPoint();
                dwellDuration = Random.Range(currentPoi.DwellTimeRange.x, currentPoi.DwellTimeRange.y);
                hasDestination = true;
            }
        }

        if (!hasDestination)
        {
            if (!territory.TryGetRandomPoint(out destination)) return Status.Failure;
            dwellDuration = Random.Range(WanderPauseMin.Value, WanderPauseMax.Value);
        }

        if (!movement.TryMoveTo(destination, false, 5f))
        {
            // Unreachable POI: put it on cooldown so we don't pick it again immediately.
            territory.MarkVisited(currentPoi);
            return Status.Failure;
        }

        travelStartTime = Time.time;
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        if (!dwelling)
        {
            if (movement.PathFailed || Time.time - travelStartTime > MaxTravelTime.Value) return Status.Failure;
            if (!movement.HasArrived) return Status.Running;

            // Arrived: stand around for a random time. currentPoi.Type is available here
            // later for drinking / sniffing / resting animations.
            dwelling = true;
            dwellEndTime = Time.time + dwellDuration;
            movement.Stop();
            territory.MarkVisited(currentPoi);
        }

        return Time.time >= dwellEndTime ? Status.Success : Status.Running;
    }

    protected override void OnEnd()
    {
        // Interrupted or failed mid-walk: don't keep following a stale path.
        if (movement != null && !dwelling) movement.Stop();
    }
}