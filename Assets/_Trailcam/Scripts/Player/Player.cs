using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerMovement))]
[RequireComponent(typeof(PlayerVision))]
[RequireComponent(typeof(Interactor))]
public class Player : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] PlayerMovement movement;
    [SerializeField] PlayerVision vision;
    [SerializeField] Interactor interactor;

    void OnMove(InputValue value)
    {
        movement.MoveInput = value.Get<Vector2>();
    }

    void OnLook(InputValue value)
    {
        vision.VisionInput = value.Get<Vector2>();
    }

    void OnSprint(InputValue value)
    {
        movement.SprintInput = value.isPressed;
    }

    void OnJump(InputValue value)
    {
        if (value.isPressed)
        {
            movement.TryJump();
        }
    }

    void OnInteract(InputValue value)
    {
        if (value.isPressed)
        {
            interactor.TryInteract();
        }
    }

    void OnValidate()
    {
        if (movement == null)
        {
            movement = GetComponent<PlayerMovement>();
        }
        if (vision == null)
        {
            vision = GetComponent<PlayerVision>();
        }
        if (interactor == null)
        {
            interactor = GetComponent<Interactor>();
        }
    }

    void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

}
