using UnityEngine;
using UnityEngine.InputSystem;

public class PauseMenuController : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] GameObject pauseMenuUI;

    [Header("Input")]
    [SerializeField] PlayerInput playerInput; // the PlayerInput component on the Player object

    public bool IsPaused { get; private set; }

    void Awake()
    {
        // Safety net: guarantees the menu is hidden at startup even if it
        // was accidentally left active in the Inspector.
        if (pauseMenuUI != null && pauseMenuUI.activeSelf)
        {
            pauseMenuUI.SetActive(false);
        }
    }

    // Unity drops the cursor lock whenever the window/Game view loses focus,
    // and a lock set in Start is ignored if the Game view isn't focused yet. Re-apply it on refocus.
    void OnApplicationFocus(bool hasFocus)
    {
        if (hasFocus && !IsPaused)
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
    }

    public void Toggle()
    {
        if (IsPaused)
        {
            Resume();
        }
        else
        {
            Pause();
        }
    }

    public void Pause()
    {
        if (IsPaused) return;
        IsPaused = true;

        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;

        // Stops OnMove/OnLook/OnSprint/OnJump/OnInteract from firing on Player,
        // without touching the separate Menu action map instance in InGameMenuInput.
        playerInput.DeactivateInput();

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void Resume()
    {
        if (!IsPaused) return;
        IsPaused = false;

        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;

        playerInput.ActivateInput();

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
}
