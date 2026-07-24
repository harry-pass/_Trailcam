using UnityEngine;

public class Ammo : MonoBehaviour, IInteractable
{
    [SerializeField] AmmoType type;
    [SerializeField] int amount = 10;

    public void Interact(Interactor interactor)
    {
        if (interactor.TryGetComponent(out Inventory inventory))
        {
            Debug.Log($"Picked up {amount} of {type}");
            inventory.AddAmmo(type, amount);
            Destroy(gameObject);
        }
    }
}
