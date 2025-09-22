using UnityEngine;

/// <summary>
/// Sistema de ataque simplificado para fantasmas que funciona sin animaciones
/// </summary>
public class SimpleGhostAttack : MonoBehaviour, IEnemyAttack
{
    [Header("Configuración del Ataque")]
    [SerializeField] private float range = 2.2f;
    [SerializeField] private float damage = 10f;
    [SerializeField] private float cooldown = 1f;
    [SerializeField] private float windup = 0.2f;
    [SerializeField] private float recover = 0.3f;
    
    private float lastAttackTime;
    private EnemyCommon enemy;
    
    // Propiedades de IEnemyAttack
    public float Cooldown => cooldown;
    public float Windup => windup;
    public float Recover => recover;
    
    void Awake()
    {
        enemy = GetComponent<EnemyCommon>();
        Debug.Log($"SimpleGhostAttack: Inicializado en {name}");
    }
    
    public bool CanAttack()
    {
        // Verificar cooldown
        if (Time.time - lastAttackTime < cooldown)
        {
            return false;
        }
        
        // Verificar target
        if (TargetRegistry.Instance == null || TargetRegistry.Instance.CurrentTarget == null)
        {
            return false;
        }
        
        var target = TargetRegistry.Instance.CurrentTarget;
        if (!target.IsValid || target.AimRoot == null)
        {
            return false;
        }
        
        // Verificar distancia
        float distance = Vector3.Distance(transform.position, target.AimRoot.position);
        return distance <= range;
    }
    
    public void DoAttack()
    {
        Debug.Log($"SimpleGhostAttack: DoAttack() ejecutado en {name}");
        Debug.Log($"   HP del fantasma antes del ataque: {(enemy != null ? enemy.CurrentHP.ToString() : "N/A")}");
        
        lastAttackTime = Time.time;
        
        // Bloquear movimiento
        if (enemy != null)
        {
            enemy.LockMotionFor(windup + recover);
        }
        
        // Aplicar daño inmediatamente (sin animación)
        ApplyDamageDirectly();
        
        Debug.Log($"   HP del fantasma después del ataque: {(enemy != null ? enemy.CurrentHP.ToString() : "N/A")}");
    }
    
    private void ApplyDamageDirectly()
    {
        Debug.Log($"SimpleGhostAttack: Aplicando daño directamente...");
        
        // Verificar target
        if (TargetRegistry.Instance == null)
        {
            Debug.LogError("SimpleGhostAttack: TargetRegistry es NULL");
            return;
        }
        
        var target = TargetRegistry.Instance.CurrentTarget;
        if (target == null)
        {
            Debug.LogError("SimpleGhostAttack: Target es NULL");
            return;
        }
        
        if (!target.IsValid)
        {
            Debug.LogError("SimpleGhostAttack: Target no es válido");
            return;
        }
        
        if (target.AimRoot == null)
        {
            Debug.LogError("SimpleGhostAttack: Target.AimRoot es NULL");
            return;
        }
        
        // Buscar IDamageable
        IDamageable damageable = (target as Component)?.GetComponentInParent<IDamageable>();
        if (damageable == null)
        {
            Debug.LogError($"SimpleGhostAttack: Target {target.AimRoot.name} no tiene IDamageable");
            
            // Listar todos los componentes
            var components = target.AimRoot.GetComponents<Component>();
            Debug.Log($"Componentes en {target.AimRoot.name}:");
            foreach (var comp in components)
            {
                Debug.Log($"  - {comp.GetType().Name}");
            }
            return;
        }
        
        // Verificar distancia
        float distance = Vector3.Distance(transform.position, target.AimRoot.position);
        if (distance > range)
        {
            Debug.LogWarning($"SimpleGhostAttack: Target muy lejos ({distance:F2} > {range:F2})");
            return;
        }
        
        // Aplicar daño
        Debug.Log($"SimpleGhostAttack: Aplicando {damage} de daño a {target.AimRoot.name}");
        damageable.TakeDamage(damage);
        Debug.Log("SimpleGhostAttack: ¡Daño aplicado correctamente!");
    }
    
    public float GetAttackDuration()
    {
        return windup + recover;
    }
    
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, range);
    }
}
