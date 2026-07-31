using TMPro;
using UnityEngine;

public class HealthUI : MonoBehaviour
{
    [SerializeField] Health health;
    [SerializeField] TextMeshProUGUI healthText;

    void OnEnable()
    {
        health.OnHealthChanged += UpdateHealthUI;
        UpdateText(health.CurrentHealth, health.MaxHealth);
    }

    void OnDisable()
    {
        health.OnHealthChanged -= UpdateHealthUI;
    }

    void UpdateHealthUI(float current, float max)
    {
        UpdateText(current, max);
    }

    void UpdateText(float current, float max)
    {
        healthText.text = $"{Mathf.Ceil(current)} / {Mathf.Ceil(max)}";
    }
}
