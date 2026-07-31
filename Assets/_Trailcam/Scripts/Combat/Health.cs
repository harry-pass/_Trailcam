using System;
using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField] private float maxHealth;
    public float MaxHealth => maxHealth;
    public float CurrentHealth { get; private set; }

    public event Action<float, float> OnHealthChanged;
    public event Action OnDeath;

    void Awake() => CurrentHealth = maxHealth;

    public void TakeDamage(float damage)
    {
        CurrentHealth = Mathf.Max(0, CurrentHealth - damage);
        OnHealthChanged?.Invoke(CurrentHealth, maxHealth);
        if (CurrentHealth <= 0)
        {
            OnDeath?.Invoke();
        }

    }
}
