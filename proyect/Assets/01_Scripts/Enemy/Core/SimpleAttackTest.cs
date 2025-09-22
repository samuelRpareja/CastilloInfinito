using UnityEngine;

/// <summary>
/// Script simple para probar el ataque sin depender de la animación
/// </summary>
public class SimpleAttackTest : MonoBehaviour
{
    [Header("Configuración")]
    [SerializeField] private float attackRange = 2.2f;
    [SerializeField] private float attackDamage = 10f;
    [SerializeField] private float attackCooldown = 1f;
    [SerializeField] private bool enableAutoAttack = true;
    
    private float lastAttackTime;
    private GhostController ghostController;
    
    void Start()
    {
        ghostController = FindObjectOfType<GhostController>();
        if (ghostController == null)
        {
            Debug.LogError("SimpleAttackTest: No se encontró GhostController");
        }
    }
    
    void Update()
    {
        if (!enableAutoAttack || ghostController == null) return;
        
        // Verificar si puede atacar
        if (Time.time - lastAttackTime < attackCooldown) return;
        
        // Verificar si hay target
        if (TargetRegistry.Instance == null || TargetRegistry.Instance.CurrentTarget == null) return;
        
        var target = TargetRegistry.Instance.CurrentTarget;
        if (!target.IsValid) return;
        
        // Calcular distancia
        float distance = Vector3.Distance(transform.position, target.AimRoot.position);
        
        // Si está en rango, atacar
        if (distance <= attackRange)
        {
            Debug.Log($"SimpleAttackTest: Atacando a distancia {distance:F2}");
            PerformAttack(target);
            lastAttackTime = Time.time;
        }
    }
    
    void PerformAttack(ITarget target)
    {
        Debug.Log("SimpleAttackTest: Ejecutando ataque simple...");
        
        // Buscar IDamageable
        IDamageable damageable = (target as Component)?.GetComponentInParent<IDamageable>();
        if (damageable == null)
        {
            Debug.LogError("SimpleAttackTest: Target no tiene IDamageable");
            return;
        }
        
        // Aplicar daño
        Debug.Log($"SimpleAttackTest: Aplicando {attackDamage} de daño");
        damageable.TakeDamage(attackDamage);
        Debug.Log("SimpleAttackTest: Daño aplicado correctamente");
    }
    
    [ContextMenu("Test Attack Now")]
    public void TestAttackNow()
    {
        if (TargetRegistry.Instance == null || TargetRegistry.Instance.CurrentTarget == null)
        {
            Debug.LogError("SimpleAttackTest: No hay target para atacar");
            return;
        }
        
        PerformAttack(TargetRegistry.Instance.CurrentTarget);
    }
    
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
