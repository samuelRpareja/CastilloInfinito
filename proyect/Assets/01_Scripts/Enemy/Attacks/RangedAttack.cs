using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedAttack : MonoBehaviour, IEnemyAttack
{
    [Header("Params")]
    [SerializeField] private float damage = 8f;
    [SerializeField] private float projectileSpeed = 18f;
    [SerializeField] private float cooldown = 1.2f;
    [SerializeField] private float maxRange = 24f;
    [SerializeField] private float spreadDegrees = 1.5f;

    [Header("Refs")]
    [SerializeField] private Transform muzzle;               // punto de disparo
    [SerializeField] private GameObject projectilePrefab;    // requiere EnemyProjectile

    private float lastTime;

    public float Cooldown => cooldown;

    private void Reset()
    {
        muzzle = transform;
    }

    public bool CanAttack()
    {
        if (Time.time < lastTime + cooldown) return false;

        var tgt = TargetRegistry.Instance?.CurrentTarget;
        if (tgt == null || !tgt.IsValid) return false;

        float dist = Vector3.Distance(muzzle ? muzzle.position : transform.position, tgt.AimRoot.position);
        return dist <= maxRange;
    }

    public void DoAttack()
    {
        lastTime = Time.time;

        var tgt = TargetRegistry.Instance?.CurrentTarget;
        if (tgt == null || !tgt.IsValid) return;

        var origin = muzzle ? muzzle.position : transform.position;
        Vector3 dir = (tgt.AimRoot.position - origin).normalized;

        // Pequeña dispersión para evitar láser perfecto
        if (spreadDegrees > 0f)
        {
            dir = Quaternion.Euler(Random.Range(-spreadDegrees, spreadDegrees),
                                   Random.Range(-spreadDegrees, spreadDegrees), 0f) * dir;
        }

        // Crear proyectil (usa pool si existe, si no Instantiate)
        GameObject go = null;

        var pool = GetComponentInParent<ProjectilePool>();
        if (pool != null)
        {
            go = pool.Spawn(projectilePrefab, origin, Quaternion.LookRotation(dir));
        }
        else
        {
            go = Instantiate(projectilePrefab, origin, Quaternion.LookRotation(dir));
        }

        var proj = go.GetComponent<EnemyProjectile>();
        if (proj != null)
        {
            proj.Launch(dir, projectileSpeed, damage, pool);

        }

        // TODO: Anim, VFX/SFX de disparo
    }
}
