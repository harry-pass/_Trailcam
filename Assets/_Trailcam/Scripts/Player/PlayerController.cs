using System.Collections;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Windows;

// Guarantees that a CharacterController component is always attached to the same GameObject as this script (safety measure)
[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("Player Movement Parameters")]
    public float MaxSpeed => SprintInput ? SprintSpeed : WalkSpeed;
    [SerializeField] float WalkSpeed = 4f;
    [SerializeField] float SprintSpeed = 8f;
    [Header("Forward/Backward Movement")]
    [SerializeField] float ForwardAcceleration = 20f;
    [SerializeField] float ForwardDeceleration = 30f;
    [SerializeField] float ForwardReversalSpeed = 12f;

    [Header("Strafing Movement")]
    [SerializeField] float StrafeAcceleration = 20f;
    [SerializeField] float StrafeDeceleration = 30f;
    [SerializeField] float StrafeReversalSpeed = 70f;

    float currentForwardSpeed;
    float currentStrafeSpeed;
    public Vector3 CurrentVelocity { get; private set; }
    public float CurrentSpeed {  get; private set; }

    [Header("Player Vision Parameters")]
    public Vector2 VisionSensitivity = new Vector2(0.1f, 0.1f);

    public float PitchLimit = 85f;

    float currentPitch = 0f;

    public float CurrentPitch
    {
        get => currentPitch;
        set
        {
            currentPitch = Mathf.Clamp(value, -PitchLimit, PitchLimit);
        }
    }

    [Header("Input")]
    public Vector2 MoveInput;
    public Vector2 VisionInput;
    public bool SprintInput;

    [Header("Components")]
    [SerializeField] CinemachineCamera fpCamera;
    [SerializeField] CharacterController controller;

    void OnValidate()
    {
        if (controller == null)
        {
            controller = GetComponent<CharacterController>();
        }
    }

    private void Update()
    {
        MoveUpdate();
        VisionUpdate();
    }


    void MoveUpdate()
    {
        Vector2 input = MoveInput;
        if (input.magnitude > 1f) input.Normalize();

        float targetForward = input.y * MaxSpeed;
        float targetStrafe = input.x * MaxSpeed;

        currentForwardSpeed = MoveAxis(currentForwardSpeed, targetForward, ForwardAcceleration, ForwardDeceleration, ForwardReversalSpeed);
        currentStrafeSpeed = MoveAxis(currentStrafeSpeed, targetStrafe, StrafeAcceleration, StrafeDeceleration, StrafeReversalSpeed);

        Vector3 forward = transform.forward; forward.y = 0f; forward.Normalize();
        Vector3 right = transform.right; right.y = 0f; right.Normalize();

        CurrentVelocity = forward * currentForwardSpeed + right * currentStrafeSpeed;
        CurrentSpeed = CurrentVelocity.magnitude;

        controller.Move(CurrentVelocity * Time.deltaTime);
    }

    float MoveAxis(float current, float target, float accel, float decel, float reversal)
    {
        bool isStopping = Mathf.Abs(target) < 0.01f;
        bool isReversing = current * target < 0f; // opposite signs = reversing on this axis
        float rate = isReversing ? reversal : (isStopping ? decel : accel);
        return Mathf.MoveTowards(current, target, rate * Time.deltaTime);
    }

    void VisionUpdate()
    {
        Vector2 input = new Vector2(VisionInput.x * VisionSensitivity.x, VisionInput.y * VisionSensitivity.y);
        
        //looking up and down
        CurrentPitch -= input.y;
        fpCamera.transform.localRotation = Quaternion.Euler(CurrentPitch, 0f, 0f);

        //looking left and right
        transform.Rotate(Vector3.up * input.x);
    }
}
