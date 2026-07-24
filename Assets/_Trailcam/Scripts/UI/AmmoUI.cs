using TMPro;
using UnityEngine;

public class AmmoUI : MonoBehaviour
{
    [SerializeField] Inventory inventory;
    [SerializeField] AmmoType trackedType;
    [SerializeField] TextMeshProUGUI ammoText;

    void OnEnable()
    {
        inventory.OnAmmoChanged += HandleAmmoChanged;
        UpdateText(inventory.GetAmmo(trackedType));
    }

    void OnDisable()
    {
        inventory.OnAmmoChanged -= HandleAmmoChanged;
    }

    void HandleAmmoChanged(AmmoType type, int newCount)
    {
        if (type != trackedType) return;
        UpdateText(newCount);
    }

    void UpdateText(int count)
    {
        ammoText.text = count.ToString();
    }
}
