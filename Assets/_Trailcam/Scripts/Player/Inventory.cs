using System;
using System.Collections.Generic;
using UnityEngine;

public enum AmmoType
{
    Pistol,
    Rifle,
    Shotgun
}

public class Inventory : MonoBehaviour
{
    Dictionary<AmmoType, int> ammoCount = new();

    public event Action<AmmoType, int> OnAmmoChanged;

    public int GetAmmo(AmmoType type) => ammoCount.TryGetValue(type, out int count) ? count : 0;

    public void AddAmmo(AmmoType type, int amount)
    {
        if (amount <= 0) return;
        ammoCount[type] = GetAmmo(type) + amount;
        OnAmmoChanged?.Invoke(type, ammoCount[type]);
    }

    public bool TryRemoveAmmo(AmmoType type, int amount)
    {
        if (GetAmmo(type) < amount) return false;
        ammoCount[type] -= amount;
        OnAmmoChanged?.Invoke(type, ammoCount[type]);
        return true;
    }

}
