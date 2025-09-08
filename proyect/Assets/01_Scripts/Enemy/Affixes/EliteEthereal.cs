using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EliteEthereal : MonoBehaviour, IEliteAffix
{
    [SerializeField] private float iframeDuration = 0.5f;
    [SerializeField] private float cooldown = 6f;

    private EnemyCommon enemy;
    private float lastProc;

    public string Name => "Ethereal";

    public void Apply(GameObject go)
    {
        enemy = go.GetComponent<EnemyCommon>();
    }

    // Este método debe llamarse dentro de TakeDamage()
    public bool TryNegateDamage()
    {
        if (Time.time < lastProc + cooldown) return false;

        lastProc = Time.time;
        // TODO: efecto visual (fade, shader, etc.)
        Debug.Log("✨ Elite Ethereal evitó daño");
        return true; // daño cancelado
    }
}
