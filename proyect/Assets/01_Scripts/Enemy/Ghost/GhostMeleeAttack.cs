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
    [SerializeField] private GhostAnimationBridge bridge; // Cambiado a GhostAnimationBridge normal
    [SerializeField] private float fallbackHitTime = 0.22f; // ≈ windup

    private float last;
    private EnemyCommon enemy;

    [Header("Referencias")]
    [SerializeField] private HitboxDamager hitbox;
    
    // Propiedades públicas para acceso desde otros scripts
    public float Cooldown => cooldown;
    public float Windup => windup;
    public float Recover => recover;
    public float Range => range;
    public float Damage => damage;

    

    private void Awake()
    {
        enemy = GetComponent<EnemyCommon>();
        if (bridge == null) bridge = GetComponentInChildren<GhostAnimationBridge>();
        if (bridge == null) bridge = GetComponent<GhostAnimationBridge>();
        
        Debug.Log($"GhostMeleeAttack: Bridge encontrado: {(bridge != null ? bridge.GetType().Name : "NULL")}");
    }

    public bool CanAttack()
    {
        Debug.Log($"<color=cyan>[GhostMeleeAttack] CanAttack() - {name}</color>");
        
        // Verificar cooldown
        if (Time.time < last + cooldown) 
        {
            float remainingCooldown = (last + cooldown) - Time.time;
            Debug.Log($"   ❌ En cooldown. Tiempo restante: {remainingCooldown:F2}s");
            return false;
        }
        Debug.Log($"   ✅ Cooldown OK");
        
        // Verificar TargetRegistry
        if (TargetRegistry.Instance == null)
        {
            Debug.LogError($"   ❌ TargetRegistry.Instance es NULL");
            return false;
        }
        Debug.Log($"   ✅ TargetRegistry existe");
        
        // Verificar target
        var t = TargetRegistry.Instance.CurrentTarget;
        if (t == null)
        {
            Debug.LogWarning($"   ❌ Target es NULL");
            return false;
        }
        Debug.Log($"   ✅ Target existe: {t.GetType().Name}");
        
        if (!t.IsValid)
        {
            Debug.LogWarning($"   ❌ Target no es válido (IsValid: {t.IsValid})");
            return false;
        }
        Debug.Log($"   ✅ Target es válido");
        
        // Verificar AimRoot
        if (t.AimRoot == null)
        {
            Debug.LogError($"   ❌ Target.AimRoot es NULL");
            return false;
        }
        Debug.Log($"   ✅ Target.AimRoot existe: {t.AimRoot.name}");
        
        // Calcular distancia
        Vector3 enemyPos = transform.position;
        Vector3 targetPos = t.AimRoot.position;
        
        float dist = use3DDistance
            ? Vector3.Distance(enemyPos, targetPos)
            : Vector3.Distance(new Vector3(enemyPos.x, 0, enemyPos.z),
                               new Vector3(targetPos.x, 0, targetPos.z));
        
        Debug.Log($"   📏 Distancia calculada: {dist:F2} (rango: {range:F2})");
        Debug.Log($"   📍 Posición enemigo: {enemyPos}");
        Debug.Log($"   📍 Posición target: {targetPos}");
        
        bool canAttack = dist <= range;
        Debug.Log($"   {(canAttack ? "✅" : "❌")} Puede atacar: {canAttack}");
        
        return canAttack;
    }

    public void DoAttack()
    {
        Debug.LogWarning($"<color=orange>[GhostMeleeAttack] DoAttack() - {name}</color>");
        Debug.Log($"   🎯 Iniciando ataque a las {Time.time:F2}s");

        last = Time.time;
        Debug.Log($"   ⏰ Tiempo de ataque actualizado: {last:F2}s");
        
        if (enemy != null) 
        {
            enemy.LockMotionFor(windup + recover);
            Debug.Log($"   🔒 Movimiento bloqueado por {windup + recover:F2}s");
        }
        else
        {
            Debug.LogError($"   ❌ Enemy es NULL");
        }
        
        if (bridge != null) 
        {
            bridge.PlayAttack();
            Debug.Log($"   🎬 Animación de ataque iniciada");
        }
        else
        {
            Debug.LogWarning($"   ⚠️ Bridge es NULL - no se puede reproducir animación");
        }
        
        CancelInvoke(nameof(ApplyDamageNow));
        Invoke(nameof(ApplyDamageNow), fallbackHitTime); // Fallback SIEMPRE
        Debug.Log($"   ⏱️ Daño programado para {fallbackHitTime:F2}s (fallback)");
        Debug.Log($"   ⏰ Tiempo actual: {Time.time:F2}s, se ejecutará a las: {Time.time + fallbackHitTime:F2}s");
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
        Debug.Log($"<color=red>[GhostMeleeAttack] ApplyDamageNow() - {name}</color>");
        Debug.Log($"   🎯 Intentando aplicar daño a las {Time.time:F2}s");
        Debug.Log($"   🔥 ¡ESTE MÉTODO SE ESTÁ EJECUTANDO! 🔥");
        
        // Verificar TargetRegistry
        Debug.Log($"   🔍 Verificando TargetRegistry...");
        if (TargetRegistry.Instance == null)
        {
            Debug.LogError($"   ❌ TargetRegistry.Instance es NULL");
            return;
        }
        Debug.Log($"   ✅ TargetRegistry existe");
        
        // Obtener target
        Debug.Log($"   🔍 Obteniendo target...");
        var t = TargetRegistry.Instance.CurrentTarget;
        if (t == null)
        {
            Debug.LogWarning($"   ❌ Target es NULL");
            return;
        }
        Debug.Log($"   ✅ Target existe: {t.GetType().Name}");
        
        Debug.Log($"   🔍 Verificando validez del target...");
        if (!t.IsValid)
        {
            Debug.LogWarning($"   ❌ Target no es válido (IsValid: {t.IsValid})");
            return;
        }
        Debug.Log($"   ✅ Target es válido");
        
        // Verificar AimRoot
        Debug.Log($"   🔍 Verificando AimRoot...");
        if (t.AimRoot == null)
        {
            Debug.LogError($"   ❌ Target.AimRoot es NULL");
            return;
        }
        Debug.Log($"   ✅ Target.AimRoot existe: {t.AimRoot.name}");
        
        // Intentar obtener el componente que puede recibir daño
        Debug.Log($"   🔍 Buscando componente IDamageable...");
        IDamageable damageableTarget = (t as Component)?.GetComponentInParent<IDamageable>();
        if (damageableTarget == null)
        {
            Debug.LogError($"   ❌ Target '{t.AimRoot.name}' no tiene componente IDamageable");
            Debug.Log($"   🔍 Buscando IDamageable en: {t.AimRoot.name}");
            
            // Buscar en todos los componentes
            var components = t.AimRoot.GetComponents<Component>();
            Debug.Log($"   📋 Componentes encontrados: {components.Length}");
            foreach (var comp in components)
            {
                Debug.Log($"      - {comp.GetType().Name}");
            }
            return;
        }
        Debug.Log($"   ✅ IDamageable encontrado: {damageableTarget.GetType().Name}");

        // Calcular distancia
        Debug.Log($"   🔍 Calculando distancia...");
        float dist = DistanceTo(t);
        Debug.Log($"   📏 Distancia al target: {dist:F2} (rango: {range:F2})");
        Debug.Log($"   📍 Posición enemigo: {transform.position}");
        Debug.Log($"   📍 Posición target: {t.AimRoot.position}");
        
        if (dist <= range)
        {
            Debug.Log($"   ✅ Target está en rango - APLICANDO DAÑO");
            Debug.Log($"   💥 Daño a aplicar: {damage}");
            
            // 👇👇 ¡ESTA ES LA LÍNEA MÁS IMPORTANTE! 👇👇
            Debug.Log($"   🔥 LLAMANDO A TakeDamage({damage})...");
            damageableTarget.TakeDamage(damage);
            Debug.Log($"   ✅ TakeDamage() completado");

            Debug.LogWarning($"   🎯 HIT! Se aplicaron {damage} de daño al objetivo @ dist={dist:0.00}");
        }
        else
        {
            Debug.LogWarning($"   ❌ MISS @ dist={dist:0.00} (rango={range})");
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
        
        // AÑADIR INVOKE PARA FALLBACK
        CancelInvoke(nameof(ApplyDamageNow));
        Invoke(nameof(ApplyDamageNow), fallbackHitTime);
        Debug.Log($"IEnemyAttack.DoAttack: Daño programado para {fallbackHitTime:F2}s");

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
