using UnityEngine;

public class Ammo : MonoBehaviour, IInteractable
{
    public void Interact()
    {
        Debug.Log("Picked up ammo!");
    }
}
