using UnityEngine;

[RequireComponent(typeof(AIMovement))]
[RequireComponent(typeof(AIPerception))]
[RequireComponent(typeof(AISight))]
[RequireComponent(typeof(AIHearing))]
public class EnemyAgent : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] AIMovement movement;
    [SerializeField] AIPerception perception;

    public AIMovement Movement => movement;
    public AIPerception Perception => perception;

    void OnValidate()
    {
        if (movement == null) movement = GetComponent<AIMovement>();
        if (perception == null) perception = GetComponent<AIPerception>();
    }
}

