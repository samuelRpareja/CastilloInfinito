using UnityEngine;

/// <summary>
/// Script que controla el sistema de ataque del personaje
/// Maneja la activación del hitbox y la sincronización con la animación
/// </summary>
public class PlayerAttackController : MonoBehaviour
{
    [Header("Configuración de Ataque")]
    [SerializeField] private float attackDamage = 10f;
    [SerializeField] private float hitboxDelay = 0.2f; // Tiempo antes de activar el hitbox
    [SerializeField] private float hitboxDuration = 0.5f; // Duración del hitbox activo
    
    [Header("Referencias")]
    [SerializeField] private PlayerHitbox hitbox;
    [SerializeField] private SimpleAttacker attacker;
    [SerializeField] private Animator animator;
    
    [Header("Debug")]
    [SerializeField] private bool showDebugInfo = true;
    
    private void Start()
    {
        // Obtener referencias si no están asignadas
        if (attacker == null)
            attacker = GetComponent<SimpleAttacker>();
        
        if (animator == null)
            animator = GetComponent<Animator>();
        
        if (hitbox == null)
            hitbox = GetComponentInChildren<PlayerHitbox>();
        
        // Verificar que tenemos todo lo necesario
        if (attacker == null)
        {
            Debug.LogError("PlayerAttackController: No se encontró SimpleAttacker");
            return;
        }
        
        if (hitbox == null)
        {
            Debug.LogError("PlayerAttackController: No se encontró PlayerHitbox");
            return;
        }
        
        // Configurar el hitbox
        hitbox.SetDamage(attackDamage);
        
        // Suscribirse a los eventos de ataque
        attacker.OnAttackStarted += OnAttackStarted;
        attacker.OnAttackEnded += OnAttackEnded;
        
        Debug.Log("PlayerAttackController: Sistema de ataque inicializado");
    }
    
    private void OnDestroy()
    {
        // Desuscribirse de los eventos
        if (attacker != null)
        {
            attacker.OnAttackStarted -= OnAttackStarted;
            attacker.OnAttackEnded -= OnAttackEnded;
        }
    }
    
    void OnAttackStarted()
    {
        Debug.Log("PlayerAttackController: Ataque iniciado");
        
        // Activar el hitbox después de un pequeño delay
        Invoke(nameof(ActivateHitbox), hitboxDelay);
        
        // Desactivar el hitbox después de la duración
        Invoke(nameof(DeactivateHitbox), hitboxDelay + hitboxDuration);
    }
    
    void OnAttackEnded()
    {
        Debug.Log("PlayerAttackController: Ataque terminado");
        
        // Asegurar que el hitbox esté desactivado
        DeactivateHitbox();
    }
    
    void ActivateHitbox()
    {
        if (hitbox != null)
        {
            hitbox.ActivateHitbox();
            Debug.Log("PlayerAttackController: Hitbox activado");
        }
    }
    
    void DeactivateHitbox()
    {
        if (hitbox != null)
        {
            hitbox.DeactivateHitbox();
            Debug.Log("PlayerAttackController: Hitbox desactivado");
        }
    }
    
    /// <summary>
    /// Configura el daño del ataque
    /// </summary>
    public void SetAttackDamage(float damage)
    {
        attackDamage = damage;
        if (hitbox != null)
        {
            hitbox.SetDamage(damage);
        }
    }
    
    /// <summary>
    /// Configura el delay del hitbox
    /// </summary>
    public void SetHitboxDelay(float delay)
    {
        hitboxDelay = delay;
    }
    
    /// <summary>
    /// Configura la duración del hitbox
    /// </summary>
    public void SetHitboxDuration(float duration)
    {
        hitboxDuration = duration;
    }
    
    void OnDrawGizmos()
    {
        if (!showDebugInfo || hitbox == null) return;
        
        // Dibujar información del ataque
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, 0.5f);
        
        // Mostrar dirección del ataque
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, transform.forward * 2f);
    }
}
