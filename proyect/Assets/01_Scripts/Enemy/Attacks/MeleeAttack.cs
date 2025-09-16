using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Ataque melee gen�rico. Aplica da�o a un IDamageable en rango.
/// Puede extenderse con OnAttack() para animaciones/VFX espec�ficos.
/// </summary>
public class MeleeAttack : MonoBehaviour, IEnemyAttack
{
    [Header("Params")]
    [SerializeField] protected float damage = 10f;
    [SerializeField] protected float range = 1.6f;
    [SerializeField] protected float cooldown = 1.0f;
    [SerializeField] protected float aimTurnSpeed = 12f;

    private float lastTime;
    protected EnemyCommon enemy;

    public float Cooldown => cooldown;

    public float Windup => throw new System.NotImplementedException();

    public float Recover => throw new System.NotImplementedException();

    float IEnemyAttack.Cooldown => throw new System.NotImplementedException();

    protected virtual void Awake()
    {
        enemy = GetComponent<EnemyCommon>();
        if (!enemy)
            Debug.LogWarning($"{name}: falta EnemyCommon en MeleeAttack");
    }

    public virtual bool CanAttack()
    {
        if (Time.time < lastTime + cooldown) return false;

        var tgt = TargetRegistry.Instance?.CurrentTarget;
        if (tgt == null || !tgt.IsValid) return false;

        var to = tgt.AimRoot.position - transform.position; to.y = 0f;
        return to.sqrMagnitude <= range * range;
    }

    public virtual void DoAttack()
    {
        lastTime = Time.time;

        var tgt = TargetRegistry.Instance?.CurrentTarget;
        if (tgt == null || !tgt.IsValid) return;

        // Rotar hacia el target
        Vector3 dir = (tgt.AimRoot.position - transform.position); dir.y = 0f;
        if (dir.sqrMagnitude > 0.0001f)
        {
            var targetRot = Quaternion.LookRotation(dir);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime * aimTurnSpeed);
        }

        // Validar rango y aplicar da�o
        if (dir.sqrMagnitude <= range * range)
        {
            if (tgt is IDamageable dmg)
            {
                dmg.TakeDamage(damage);

                // Notificar affix vamp�rico si existe
                var vamp = GetComponent<EliteVampiric>();
                if (vamp != null) vamp.OnDealDamage(damage);
            }
        }

        // Hook para clases hijas (ej: SkeletonMeleeAttack)
        OnAttack();
    }

    /// <summary>
    /// M�todo protegido que las subclases pueden sobrescribir para a�adir animaciones/VFX.
    /// </summary>
    protected virtual void OnAttack() { }

    bool IEnemyAttack.CanAttack()
    {
        throw new System.NotImplementedException();
    }

    void IEnemyAttack.DoAttack()
    {
        throw new System.NotImplementedException();
    }

    float IEnemyAttack.GetAttackDuration()
    {
        throw new System.NotImplementedException();
    }
}
