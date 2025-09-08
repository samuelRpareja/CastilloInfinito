using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EliteVampiric : MonoBehaviour, IEliteAffix
{
    [SerializeField] private float lifestealPercent = 0.05f; // 5%

    private EnemyCommon enemy;

    public string Name => "Vampiric";

    public void Apply(GameObject go)
    {
        enemy = go.GetComponent<EnemyCommon>();
        if (enemy == null)
            Debug.LogWarning("EnemyCommon faltante en Vampiric!");
    }

    // Este método debe llamarse cuando el enemigo hace daño
    public void OnDealDamage(float dmg)
    {
        if (enemy == null) return;
        float heal = dmg * lifestealPercent;
        enemy.TakeDamage(-heal); // daño negativo = curar
    }
}
