using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EliteExplosive : MonoBehaviour, IEliteAffix
{
    [SerializeField] private float explosionRadius = 4f;
    [SerializeField] private float explosionDamage = 20f;

    private EnemyCommon enemy;
    private bool subscribed;

    public string Name => "Explosive";

    public void Apply(GameObject go)
    {
        enemy = go.GetComponent<EnemyCommon>();
        if (enemy != null && !subscribed)
        {
            enemy.OnDeath += Explode;
            subscribed = true;
        }
    }

    private void Explode()
    {
        Collider[] hits = Physics.OverlapSphere(enemy.transform.position, explosionRadius);
        foreach (var hit in hits)
        {
            if (hit.TryGetComponent<IDamageable>(out var dmg))
            {
                dmg.TakeDamage(explosionDamage);
            }
        }

        // TODO: Instanciar VFX/SFX de explosión
        Debug.Log("💥 Elite Explosive explotó");
    }
}
