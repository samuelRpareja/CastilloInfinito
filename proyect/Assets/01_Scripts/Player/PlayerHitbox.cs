using UnityEngine;

/// <summary>
/// Script que maneja el hitbox del ataque del personaje
/// Se debe colocar en un GameObject hijo del personaje con un Collider como Trigger
/// </summary>
public class PlayerHitbox : MonoBehaviour
{
    [Header("Configuración de Daño")]
    [SerializeField] private float damageAmount = 10f;
    [SerializeField] private bool canHitMultipleEnemies = false;
    [SerializeField] private float hitCooldown = 0.1f; // Tiempo mínimo entre hits
    
    [Header("Debug")]
    [SerializeField] private bool showDebugInfo = true;
    
    private float lastHitTime = 0f;
    private PlayerAttackDamage parentAttackDamage;
    
    void Start()
    {
        // Obtener referencia al script padre de daño
        parentAttackDamage = GetComponentInParent<PlayerAttackDamage>();
        
        // Asegurar que este objeto tenga un Collider como Trigger
        Collider hitboxCollider = GetComponent<Collider>();
        if (hitboxCollider == null)
        {
            Debug.LogError("PlayerHitbox: No se encontró Collider en el hitbox");
            return;
        }
        
        if (!hitboxCollider.isTrigger)
        {
            Debug.LogWarning("PlayerHitbox: El Collider no está configurado como Trigger");
            hitboxCollider.isTrigger = true;
        }
        
        Debug.Log("PlayerHitbox: Hitbox del personaje inicializado");
    }
    
    void OnTriggerEnter(Collider other)
    {
        // Verificar si es un enemigo
        if (IsEnemy(other.gameObject))
        {
            TryDamageEnemy(other.gameObject);
        }
    }
    
    void OnTriggerStay(Collider other)
    {
        // Solo procesar si puede golpear múltiples enemigos
        if (canHitMultipleEnemies && IsEnemy(other.gameObject))
        {
            TryDamageEnemy(other.gameObject);
        }
    }
    
    bool IsEnemy(GameObject obj)
    {
        // Verificar si tiene el componente EnemyCommon
        EnemyCommon enemy = obj.GetComponent<EnemyCommon>();
        return enemy != null && !enemy.IsDead;
    }
    
    void TryDamageEnemy(GameObject enemyObj)
    {
        // Verificar cooldown
        if (Time.time - lastHitTime < hitCooldown)
        {
            return;
        }
        
        // Obtener el componente EnemyCommon
        EnemyCommon enemy = enemyObj.GetComponent<EnemyCommon>();
        if (enemy == null) return;
        
        // Aplicar daño
        enemy.TakeDamage(damageAmount);
        lastHitTime = Time.time;
        
        Debug.Log($"PlayerHitbox: Golpeó a {enemy.name} por {damageAmount} de daño");
        
        // Si no puede golpear múltiples enemigos, desactivar el hitbox
        if (!canHitMultipleEnemies)
        {
            gameObject.SetActive(false);
        }
    }
    
    /// <summary>
    /// Activa el hitbox para el ataque
    /// </summary>
    public void ActivateHitbox()
    {
        gameObject.SetActive(true);
        lastHitTime = 0f; // Resetear cooldown
    }
    
    /// <summary>
    /// Desactiva el hitbox
    /// </summary>
    public void DeactivateHitbox()
    {
        gameObject.SetActive(false);
    }
    
    /// <summary>
    /// Configura el daño del hitbox
    /// </summary>
    public void SetDamage(float damage)
    {
        damageAmount = damage;
    }
    
    void OnDrawGizmos()
    {
        if (!showDebugInfo) return;
        
        // Dibujar el hitbox
        Gizmos.color = new Color(1f, 0f, 0f, 0.3f);
        Collider hitboxCollider = GetComponent<Collider>();
        if (hitboxCollider != null)
        {
            Gizmos.DrawCube(transform.position, hitboxCollider.bounds.size);
        }
    }
}
