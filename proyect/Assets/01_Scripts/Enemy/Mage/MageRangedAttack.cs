using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MageRangedAttack : MonoBehaviour, IEnemyAttack
{
    [SerializeField] private float damage = 10f;
    [SerializeField] private float projectileSpeed = 15f;
    [SerializeField] private float cooldown = 1.2f;
    [SerializeField] private float maxRange = 20f;

    [Header("Refs")]
    [SerializeField] private Transform mouth; // empty en la boca
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private ProjectilePool pool;

    private float lastTime;
    public float Cooldown => cooldown;

    public bool CanAttack()
    {
        if (Time.time < lastTime + cooldown) return false;
        var tgt = TargetRegistry.Instance?.CurrentTarget;
        if (tgt == null || !tgt.IsValid) return false;

        float dist = Vector3.Distance(mouth.position, tgt.AimRoot.position);
        return dist <= maxRange;
    }

    public void DoAttack()
    {
        lastTime = Time.time;
        var tgt = TargetRegistry.Instance?.CurrentTarget;
        if (tgt == null || !tgt.IsValid) return;

        Vector3 dir = (tgt.AimRoot.position - mouth.position).normalized;

        GameObject go = pool != null
            ? pool.Spawn(projectilePrefab, mouth.position, Quaternion.LookRotation(dir))
            : Instantiate(projectilePrefab, mouth.position, Quaternion.LookRotation(dir));

        var proj = go.GetComponent<EnemyProjectile>();
        if (proj != null)
            proj.Launch(dir, projectileSpeed, damage, pool);
    }
}
