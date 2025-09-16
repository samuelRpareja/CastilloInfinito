using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EliteSwift : MonoBehaviour, IEliteAffix
{
    [SerializeField] private float speedMultiplier = 1.25f;
    [SerializeField] private float maxHpMultiplier = 0.9f; // -10% vida

    private EnemyCommon enemy;

    public string Name => "Swift";

    public void Apply(GameObject go)
    {
        enemy = go.GetComponent<EnemyCommon>();
        if (enemy == null)
        {
            Debug.LogWarning("EliteSwift: EnemyCommon faltante en " + go.name);
            return;
        }

        // Aumenta velocidad y reduce HP máximo de forma segura
        enemy.MultiplyMoveSpeed(speedMultiplier);
        enemy.ApplyMaxHpMultiplier(maxHpMultiplier);
    }
}
