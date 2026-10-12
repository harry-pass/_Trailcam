using UnityEngine;

public enum POIType { Water, Den, Tree, Other }

public class TerritoryPOI : MonoBehaviour
{
    public POIType Type = POIType.Other;
    [Min(0f)] public float Weight = 1f;                           // higher = visited more often
    public Vector2 DwellTimeRange = new Vector2(5f, 15f);         // min/max seconds spent here
    [Min(0f)] public float ArrivalRadius = 2f;                    // bear stops at a random spot this close

    public Vector3 GetArrivalPoint()
    {
        Vector2 offset = Random.insideUnitCircle * ArrivalRadius;
        return transform.position + new Vector3(offset.x, 0f, offset.y);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, Mathf.Max(ArrivalRadius, 0.25f));
    }
}
