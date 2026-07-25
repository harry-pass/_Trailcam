using UnityEngine;

public interface IInteractable
{
    public void Interact(Interactor interactor);
}

public class Interactor : MonoBehaviour
{
    public Transform InteractorSource;
    public float InteractRange;
    public LayerMask InteractLayer;
    public LayerMask BlockingMask;

    public void TryInteract()
    {
        Ray r = new Ray(InteractorSource.position, InteractorSource.forward);
        if (Physics.Raycast(r, out RaycastHit hitInfo, InteractRange, BlockingMask))
        {
            if (((1 << hitInfo.collider.gameObject.layer) & InteractLayer) != 0)
            {
                if (hitInfo.collider.gameObject.TryGetComponent(out IInteractable interactObj))
                { 
                    interactObj.Interact(this);
                }
            }
            
        }
    }
}
