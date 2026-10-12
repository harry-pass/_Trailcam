using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BearTerritory : MonoBehaviour
{
    [SerializeField] float radius = 60f;
    [SerializeField] float revisitCooldown = 120f;     // seconds before the same POI is eligible again
    [SerializeField] List<TerritoryPOI> pois = new List<TerritoryPOI>();

    readonly Dictionary<TerritoryPOI, float> lastVisited = new Dictionary<TerritoryPOI, float>();

    public float Radius => radius;

    void Awake()
    {
        if (pois.Count == 0) pois.AddRange(GetComponentsInChildren<TerritoryPOI>());
    }

    bool IsEligible(TerritoryPOI poi)
    {
        if (poi == null || poi.Weight <= 0f) return false;
        return !lastVisited.TryGetValue(poi, out float t) || Time.time - t >= revisitCooldown;
    }

    /// <summary>Weighted random POI, skipping recently visited ones. Null if none are eligible.</summary>
    public TerritoryPOI PickPOI()
    {
        float total = 0f;
        foreach (var poi in pois) if (IsEligible(poi)) total += poi.Weight;
        if (total <= 0f) return null;

        float roll = Random.value * total;
        foreach (var poi in pois)
        {
            if (!IsEligible(poi)) continue;
            roll -= poi.Weight;
            if (roll <= 0f) return poi;
        }
        return null;
    }

    public void MarkVisited(TerritoryPOI poi)
    {
        if (poi != null) lastVisited[poi] = Time.time;
    }

    /// <summary>Random walkable point inside the territory circle.</summary>
    public bool TryGetRandomPoint(out Vector3 point)
    {
        for (int i = 0; i < 8; i++)
        {
            Vector2 c = Random.insideUnitCircle * radius;
            Vector3 candidate = transform.position + new Vector3(c.x, 0f, c.y);
            if (NavMesh.SamplePosition(candidate, out NavMeshHit hit, 10f, NavMesh.AllAreas))
            {
                point = hit.position;
                return true;
            }
        }
        point = default;
        return false;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = new Color(0.2f, 0.8f, 0.3f, 0.6f);
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}