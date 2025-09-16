using UnityEngine;

[DisallowMultipleComponent]
public class GhostMeleeAttack : MonoBehaviour, IEnemyAttack
{
    [Header("Configuración del Ataque")]

    [SerializeField] private float attackDuration = 0.8f; // Duración total de la animación


    [Header("Daño")]

    [SerializeField] private float range = 2.2f;
    [SerializeField] private float damage = 10f;    // súbelo un poco para probar

    [Header("Tempos")]
    [SerializeField] private float cooldown = 1.0f;
    [SerializeField] private float windup = 0.20f;
    [SerializeField] private float recover = 0.30f;
    private float _lastAttackTime;

    [Header("Chequeo de distancia")]
    [Tooltip("ON = usa distancia 3D (recomendado para volador)")]
    [SerializeField] private bool use3DDistance = true;

    [Header("Anim / Fallback")]
    [SerializeField] private GhostAnimationBridgeExternal bridge; // o GhostAnimationBridge si usas el normal
    [SerializeField] private float fallbackHitTime = 0.22f; // ≈ windup

    private float last;
    private EnemyCommon enemy;

    [Header("Referencias")]
    [SerializeField] private HitboxDamager hitbox;
    public float Cooldown => cooldown;

    public float Windup => windup;
    public float Recover => recover;

    

    private void Awake()
    {
        enemy = GetComponent<EnemyCommon>();
        if (bridge == null) bridge = GetComponentInChildren<GhostAnimationBridgeExternal>();
        if (bridge == null) bridge = GetComponent<GhostAnimationBridgeExternal>();
    }

    public bool CanAttack()
    {
        if (Time.time < last + cooldown) return false;
        var t = TargetRegistry.Instance != null ? TargetRegistry.Instance.CurrentTarget : null;
        if (t == null || !t.IsValid) return false;
        float dist = use3DDistance
            ? Vector3.Distance(transform.position, t.AimRoot.position)
            : Vector3.Distance(new Vector3(transform.position.x, 0, transform.position.z),
                               new Vector3(t.AimRoot.position.x, 0, t.AimRoot.position.z));
        return dist <= range;
    }

    public void DoAttack()
    {
        Debug.LogWarning("<color=orange>[GhostMeleeAttack]</color> ¡DoAttack() EJECUTADO!");

        last = Time.time;
        if (enemy != null) enemy.LockMotionFor(windup + recover);
        if (bridge != null) bridge.PlayAttack();
        CancelInvoke(nameof(ApplyDamageNow));
        Invoke(nameof(ApplyDamageNow), fallbackHitTime); // Fallback SIEMPRE
    }

    // Llamado desde Animation Event (o Relay)
    public void Anim_MeleeHit()
    {
        var t = TargetRegistry.Instance?.CurrentTarget;
        if (t == null || !t.IsValid) return;

        var to = t.AimRoot.position - transform.position; to.y = 0f;
        if (to.sqrMagnitude <= range * range && t is IDamageable dmg)
            dmg.TakeDamage(damage);
        ApplyDamageNow();
    }



    // En GhostMeleeAttack.cs

    private void ApplyDamageNow()
    {
        var t = TargetRegistry.Instance != null ? TargetRegistry.Instance.CurrentTarget : null;
        if (t == null || !t.IsValid) return;

        // Intenta obtener el componente que puede recibir daño del objeto golpeado
        IDamageable damageableTarget = (t as Component)?.GetComponentInParent<IDamageable>();
        if (damageableTarget == null)
        {
            // Esto puede pasar si el target no tiene un script que implemente IDamageable
            Debug.LogWarning($"[Ghost] HIT, pero el objetivo '{t.AimRoot.name}' no tiene componente IDamageable.");
            return;
        }

        float dist = DistanceTo(t);
        if (dist <= range)
        {
            // 👇👇 ¡ESTA ES LA LÍNEA MÁS IMPORTANTE! 👇👇
            damageableTarget.TakeDamage(damage);

            Debug.LogWarning($"[Ghost] HIT! Se aplicaron {damage} de daño al objetivo @ dist={dist:0.00}");
        }
        else
        {
            Debug.Log($"[Ghost] MISS @ dist={dist:0.00} (rango={range})");
        }
    }

    private float DistanceTo(ITarget t)
    {
        Vector3 a = transform.position;
        Vector3 b = t.AimRoot.position;
        if (use3DDistance) return Vector3.Distance(a, b);
        a.y = 0f; b.y = 0f; return Vector3.Distance(a, b); // opcional horizontal
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, range);
    }
#endif
    bool IEnemyAttack.CanAttack()
    {
        // Rellenamos la lógica que faltaba:
        if (Time.time < _lastAttackTime + cooldown) return false;

        var target = enemy.Target;
        if (target == null || !target.IsValid) return false;

        return Vector3.Distance(transform.position, target.AimRoot.position) <= range;
    }

    void IEnemyAttack.DoAttack()
    {
        // Rellenamos la lógica que faltaba:
        _lastAttackTime = Time.time;
        enemy.LockMotionFor(attackDuration);

        // Llamamos a la animación a través del bridge
        if (bridge != null) bridge.PlayAttack();

        // AÑADE ESTA LÍNEA PARA CONFIRMAR
        Debug.LogError("--- DoAttack ejecutado. El problema está en la ANIMACIÓN o en el HITBOX. ---");
    }

    float IEnemyAttack.GetAttackDuration()
    {
        // Rellenamos la lógica que faltaba:
        return attackDuration;
    }

    // --- Funciones para Eventos de Animación ---

    public void OpenDamageWindow()
    {
        if (hitbox != null)
        {
            hitbox.SetDamage(damage);
            hitbox.gameObject.SetActive(true);
        }
    }

    public void CloseDamageWindow()
    {
        if (hitbox != null)
        {
            hitbox.gameObject.SetActive(false);
        }
    }
}
