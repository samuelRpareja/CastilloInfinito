using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostMeleeAttack : MonoBehaviour, IEnemyAttack
{
    [SerializeField] private float damage = 8f;
    [SerializeField] private float range = 1.5f;
    [SerializeField] private float cooldown = 1.5f;

    private float lastTime;
    public float Cooldown => cooldown;

    public bool CanAttack()
    {
        if (Time.time < lastTime + cooldown) return false;

        var tgt = TargetRegistry.Instance?.CurrentTarget;
        if (tgt == null || !tgt.IsValid) return false;

        Vector3 to = tgt.AimRoot.position - transform.position;
        to.y = 0f;
        return to.sqrMagnitude <= range * range;
    }

    public void DoAttack()
    {
        lastTime = Time.time;
        var tgt = TargetRegistry.Instance?.CurrentTarget;
        if (tgt == null || !tgt.IsValid) return;

        if (tgt is IDamageable dmg)
        {
            dmg.TakeDamage(damage);
            // aquí podrías aplicar un “slow” extra con status effect
        }
    }
}
