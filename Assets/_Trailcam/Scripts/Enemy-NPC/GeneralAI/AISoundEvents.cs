using System;
using UnityEngine;

public enum SoundCategory
{
    Footstep,
    Gunshot,
    Impact,
    Voice,
    Interaction
}

public readonly struct AISoundEvent
{
    public readonly Vector3 Position;
    public readonly float Radius;
    public readonly SoundCategory Category;
    public readonly GameObject Source;

    public AISoundEvent(Vector3 position, float radius, SoundCategory category, GameObject source)
    {
        Position = position;
        Radius = radius;
        Category = category;
        Source = source;
    }
}

/// <summary>
/// Lightweight global event bus for AI hearing. Anything that makes a noise the world
/// should be able to react to (footsteps, gunshots, breaking objects, etc.) calls
/// AISoundEvents.Emit(...) instead of talking to individual listeners directly.
/// </summary>
public static class AISoundEvents
{
    public static event Action<AISoundEvent> OnSoundEmitted;

    public static void Emit(Vector3 position, float radius, SoundCategory category, GameObject source = null)
    {
        OnSoundEmitted?.Invoke(new AISoundEvent(position, radius, category, source));
    }
}

