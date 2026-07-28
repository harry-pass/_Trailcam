using UnityEngine;
using UnityEngine.InputSystem;

public class InGameMenuInput : MonoBehaviour, PlayerInputActions.IMenuActions
{
    [SerializeField] PauseMenuController pauseMenu;
    [SerializeField] InventoryController inventory;

    PlayerInputActions inputActions;

    void Awake()
    {
        inputActions = new PlayerInputActions();
        inputActions.Menu.AddCallbacks(this);
    }

    void OnEnable()
    {
        inputActions.Menu.Enable();
    }

    void OnDisable()
    {
        inputActions.Menu.Disable();
    }

    void OnDestroy()
    {
        inputActions.Menu.RemoveCallbacks(this);
        inputActions.Dispose();
    }

    public void OnPauseMenuOpen(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            pauseMenu.Toggle();
        }
    }

    public void OnInventoryMenuOpen(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            //inventory.Toggle();
        }
    }
}
