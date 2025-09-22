using UnityEngine;

/// <summary>
/// Script para debuggear el sistema de daño y encontrar por qué el fantasma se muere
/// </summary>
public class DamageDebugger : MonoBehaviour
{
    [Header("Configuración")]
    [SerializeField] private bool enableDebug = true;
    [SerializeField] private float debugInterval = 1f;
    
    private float lastDebugTime;
    private GhostController ghostController;
    private EnemyCommon enemyCommon;
    private PlayerProxy playerProxy;
    
    void Start()
    {
        ghostController = FindObjectOfType<GhostController>();
        if (ghostController != null)
        {
            enemyCommon = ghostController.GetComponent<EnemyCommon>();
        }
        
        playerProxy = FindObjectOfType<PlayerProxy>();
    }
    
    void Update()
    {
        if (!enableDebug) return;
        
        if (Time.time - lastDebugTime >= debugInterval)
        {
            DebugDamageStatus();
            lastDebugTime = Time.time;
        }
    }
    
    void DebugDamageStatus()
    {
        Debug.Log("=== DAMAGE DEBUGGER ===");
        
        // Información del fantasma
        if (enemyCommon != null)
        {
            Debug.Log($"👻 Fantasma: {enemyCommon.name}");
            Debug.Log($"   - HP: {enemyCommon.CurrentHP}/{enemyCommon.MaxHP}");
            Debug.Log($"   - Muerto: {enemyCommon.IsDead}");
        }
        else
        {
            Debug.LogError("❌ No se encontró EnemyCommon");
        }
        
        // Información del jugador
        if (playerProxy != null)
        {
            Debug.Log($"👤 Jugador: {playerProxy.name}");
            Debug.Log($"   - HP: {playerProxy.CurrentHP}/{playerProxy.MaxHP}");
            Debug.Log($"   - Muerto: {playerProxy.IsDead}");
            Debug.Log($"   - Válido: {playerProxy.IsValid}");
        }
        else
        {
            Debug.LogError("❌ No se encontró PlayerProxy");
        }
        
        // Verificar si hay algún sistema de daño recíproco
        var hitboxDamager = FindObjectOfType<HitboxDamager>();
        if (hitboxDamager != null)
        {
            Debug.Log($"⚠️ HitboxDamager encontrado: {hitboxDamager.name}");
            Debug.Log($"   - Activo: {hitboxDamager.gameObject.activeInHierarchy}");
        }
        
        // Verificar si hay algún ataque del jugador
        var playerAttacker = FindObjectOfType<SimpleAttacker>();
        if (playerAttacker != null)
        {
            Debug.Log($"⚠️ SimpleAttacker encontrado: {playerAttacker.name}");
            Debug.Log($"   - Atacando: {playerAttacker.IsAttacking}");
        }
        
        Debug.Log("=== FIN DAMAGE DEBUGGER ===");
    }
    
    [ContextMenu("Test Damage Application")]
    public void TestDamageApplication()
    {
        Debug.Log("=== PROBANDO APLICACIÓN DE DAÑO ===");
        
        if (enemyCommon != null)
        {
            Debug.Log($"Fantasma HP antes: {enemyCommon.CurrentHP}");
            enemyCommon.TakeDamage(5f);
            Debug.Log($"Fantasma HP después: {enemyCommon.CurrentHP}");
        }
        
        if (playerProxy != null)
        {
            Debug.Log($"Jugador HP antes: {playerProxy.CurrentHP}");
            playerProxy.TakeDamage(5f);
            Debug.Log($"Jugador HP después: {playerProxy.CurrentHP}");
        }
        
        Debug.Log("=== PRUEBA COMPLETADA ===");
    }
}
