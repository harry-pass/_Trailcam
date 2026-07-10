using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerController))]
public class Player : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] PlayerController controller;

    void OnMove(InputValue value)
    {
        controller.MoveInput = value.Get<Vector2>();
    }

    void OnLook(InputValue value)
    {
        controller.VisionInput = value.Get<Vector2>();
    }

    void OnSprint(InputValue value)
    {
        controller.SprintInput = value.isPressed;
    }

    void OnJump(InputValue value)
    {
        if (value.isPressed)
        {
            Debug.Log("Jump clicked.");
            controller.TryJump();
        }
    }

    void OnValidate()
    {
        if (controller == null)
        {
            controller = GetComponent<PlayerController>();
        }
    }

    void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

}
