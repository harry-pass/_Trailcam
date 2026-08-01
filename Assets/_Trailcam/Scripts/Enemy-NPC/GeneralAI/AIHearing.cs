using UnityEngine;

/// <summary>
/// Listens to the global AISoundEvents bus and remembers the most recent sound that
/// was loud enough, and close enough, to notice. Call TryConsumeHeardSound() once per
/// perception update - it clears the flag so the same sound isn't reacted to forever.
/// </summary>
public class AIHearing : MonoBehaviour
{
    [Header("AI Hearing Parameters")]
    [SerializeField] float HearingSensitivity = 1f; // multiplies the incoming sound's radius
    [SerializeField] string ListenForTag = "Player";

    public bool HasHeardSound { get; private set; }
    public Vector3 LastHeardPosition { get; private set; }

    void OnEnable()
    {
        AISoundEvents.OnSoundEmitted += HandleSoundEmitted;
    }

    void OnDisable()
    {
        AISoundEvents.OnSoundEmitted -= HandleSoundEmitted;
    }

    void HandleSoundEmitted(AISoundEvent soundEvent)
    {
        if (!string.IsNullOrEmpty(ListenForTag) && soundEvent.Source != null && !soundEvent.Source.CompareTag(ListenForTag))
        {
            return;
        }

        float distance = Vector3.Distance(transform.position, soundEvent.Position);
        float effectiveRadius = soundEvent.Radius * HearingSensitivity;

        if (distance <= effectiveRadius)
        {
            LastHeardPosition = soundEvent.Position;
            HasHeardSound = true;
        }
    }

    public bool TryConsumeHeardSound(out Vector3 position)
    {
        position = LastHeardPosition;
        bool result = HasHeardSound;
        HasHeardSound = false;
        return result;
    }
}

