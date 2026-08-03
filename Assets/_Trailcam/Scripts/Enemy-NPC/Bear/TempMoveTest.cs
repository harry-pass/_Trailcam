using UnityEngine;
using UnityEngine.InputSystem;

public class TempMoveTest : MonoBehaviour
{
    [SerializeField] AIMovement movement;
    [SerializeField] Transform destination;

    void Update()
    {
        if (Keyboard.current != null && Keyboard.current.tKey.wasPressedThisFrame)
        {
            movement.MoveTo(destination.position, true);
        }
    }
}
