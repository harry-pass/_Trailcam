using UnityEngine;

[RequireComponent(typeof(Inventory))]
public class Weapon : MonoBehaviour
{
    [Header("Weapon Parameters")]
    public Transform FireSource;
    public float Range = 50f;
    public LayerMask HitMask = ~0;
    public AmmoType AmmoType = AmmoType.Pistol;
    public int AmmoPerShot = 1;

    [Header("Crosshair")]
    public bool ShowCrosshair = true;
    public float CrosshairSize = 6f;
    public Color CrosshairColor = Color.white;

    [Header("Components")]
    [SerializeField] Inventory inventory;

    void OnValidate()
    {
        if (inventory == null)
        {
            inventory = GetComponent<Inventory>();
        }
    }

    public void TryFire()
    {
        if (FireSource == null)
        {
            Debug.LogWarning("[Weapon] FireSource is not assigned", this);
            return;
        }

        if (!inventory.TryRemoveAmmo(AmmoType, AmmoPerShot))
        {
            Debug.Log("[Weapon] Out of ammo");
            return;
        }

        if (!TryGetHit(out RaycastHit hitInfo))
        {
            Debug.Log("[Weapon] Shot missed (nothing in range)");
            return;
        }

        EnemyAgent enemy = hitInfo.collider.GetComponentInParent<EnemyAgent>();
        if (enemy == null)
        {
            Debug.Log($"[Weapon] Hit {hitInfo.collider.name} (layer {LayerMask.LayerToName(hitInfo.collider.gameObject.layer)}) at {hitInfo.distance:F1}m - not an enemy", hitInfo.collider);
            return;
        }

        Debug.Log($"[Weapon] Hit enemy {enemy.name} at {hitInfo.distance:F1}m - destroying", enemy);
        Destroy(enemy.gameObject);
    }

    //closest hit along the aim ray, ignoring the player's own colliders
    //triggers only count if they belong to an enemy (the bear's collider is a trigger)
    bool TryGetHit(out RaycastHit closest)
    {
        closest = default;
        Ray r = new Ray(FireSource.position, FireSource.forward);
        RaycastHit[] hits = Physics.RaycastAll(r, Range, HitMask, QueryTriggerInteraction.Collide);

        bool found = false;
        foreach (RaycastHit hit in hits)
        {
            if (hit.collider.transform.IsChildOf(transform)) continue;
            if (hit.collider.isTrigger && hit.collider.GetComponentInParent<EnemyAgent>() == null) continue;
            if (!found || hit.distance < closest.distance)
            {
                closest = hit;
                found = true;
            }
        }
        return found;
    }

    //simple screen-centre crosshair, visible in the Game view
    void OnGUI()
    {
        if (!ShowCrosshair) return;

        Vector2 centre = new Vector2(Screen.width / 2f, Screen.height / 2f);
        Color previous = GUI.color;
        GUI.color = CrosshairColor;
        GUI.DrawTexture(new Rect(centre.x - CrosshairSize, centre.y - 1f, CrosshairSize * 2f, 2f), Texture2D.whiteTexture);
        GUI.DrawTexture(new Rect(centre.x - 1f, centre.y - CrosshairSize, 2f, CrosshairSize * 2f), Texture2D.whiteTexture);
        GUI.color = previous;
    }

    //aim ray in the Scene view: red when over an enemy, yellow otherwise
    void OnDrawGizmos()
    {
        if (FireSource == null) return;

        Ray r = new Ray(FireSource.position, FireSource.forward);
        if (TryGetHit(out RaycastHit hitInfo))
        {
            Gizmos.color = hitInfo.collider.GetComponentInParent<EnemyAgent>() != null ? Color.red : Color.yellow;
            Gizmos.DrawLine(r.origin, hitInfo.point);
            Gizmos.DrawWireSphere(hitInfo.point, 0.15f);
        }
        else
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(r.origin, r.origin + r.direction * Range);
        }
    }
}
