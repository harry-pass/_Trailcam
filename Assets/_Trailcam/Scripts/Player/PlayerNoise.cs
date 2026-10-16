using UnityEngine;

public class PlayerNoise : MonoBehaviour
{
    [Header("Noise radius (metres)")]
    [SerializeField] float crouchRadius = 3f;
    [SerializeField] float walkRadius = 8f;
    [SerializeField] float runRadius = 15f;

    [Header("Speed (m/s)")]
    [SerializeField] float movingThreshold = 0.5f;   // below this you're standing still and silent
    [SerializeField] float runThreshold = 4f;
    [SerializeField] float stepInterval = 0.5f;

    public bool IsCrouching { get; set; }            // set this from your movement script later

    Vector3 lastPosition;
    float nextStepTime;

    void Start() => lastPosition = transform.position;

    void Update()
    {
        Vector3 delta = transform.position - lastPosition;
        lastPosition = transform.position;
        delta.y = 0f;                                 // ignore falling and jumping
        float speed = delta.magnitude / Mathf.Max(Time.deltaTime, 0.0001f);

        if (speed < movingThreshold || Time.time < nextStepTime) return;

        float radius = IsCrouching ? crouchRadius : (speed >= runThreshold ? runRadius : walkRadius);
        AISoundEvents.Emit(transform.position, radius, SoundCategory.Footstep, gameObject);
        nextStepTime = Time.time + stepInterval;
    }
}
