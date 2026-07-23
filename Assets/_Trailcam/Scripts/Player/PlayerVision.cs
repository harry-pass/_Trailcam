using Unity.Cinemachine;
using UnityEngine;

public class PlayerVision : MonoBehaviour
{
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
    public Vector2 VisionInput;

    [Header("Components")]
    [SerializeField] CinemachineCamera fpCamera;

    void OnValidate()
    {
        if (fpCamera == null)
        {
            fpCamera = GetComponent<CinemachineCamera>();
        }
    }

    private void Update()
    {
        VisionUpdate();
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
