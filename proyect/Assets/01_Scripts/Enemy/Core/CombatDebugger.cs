using UnityEngine;

/// <summary>
/// Script que monitorea constantemente el estado del combate y muestra información detallada
/// </summary>
public class CombatDebugger : MonoBehaviour
{
    [Header("Configuración")]
    [SerializeField] private bool enableDebug = true;
    [SerializeField] private float debugInterval = 1f;
    [SerializeField] private bool showDetailedInfo = true;
    
    private float lastDebugTime;
    private GhostController ghostController;
    private IEnemyAttack ghostAttack;
    private EnemyCommon enemyCommon;
    
    void Start()
    {
        Debug.Log("CombatDebugger: Iniciando...");
        
        // Buscar componentes
        ghostController = FindObjectOfType<GhostController>();
        if (ghostController != null)
        {
            Debug.Log($"CombatDebugger: GhostController encontrado - {ghostController.name}");
            ghostAttack = ghostController.GetComponent<IEnemyAttack>();
            enemyCommon = ghostController.GetComponent<EnemyCommon>();
            
            if (ghostAttack != null)
            {
                Debug.Log($"CombatDebugger: Sistema de ataque encontrado - {ghostAttack.GetType().Name}");
            }
            else
            {
                Debug.LogWarning("CombatDebugger: No se encontró sistema de ataque (IEnemyAttack)");
            }
            
            if (enemyCommon != null)
            {
                Debug.Log($"CombatDebugger: EnemyCommon encontrado - {enemyCommon.name}");
            }
            else
            {
                Debug.LogError("CombatDebugger: No se encontró EnemyCommon");
            }
        }
        else
        {
            Debug.LogError("CombatDebugger: No se encontró GhostController");
        }
    }
    
    void Update()
    {
        if (!enableDebug) return;
        
        if (Time.time - lastDebugTime >= debugInterval)
        {
            DebugCombatStatus();
            lastDebugTime = Time.time;
        }
    }
    
    private void DebugCombatStatus()
    {
        if (!showDetailedInfo) return;
        
        Debug.Log("=== COMBAT DEBUGGER ===");
        
        // Información del fantasma
        if (ghostController != null)
        {
            Debug.Log($"👻 Fantasma: {ghostController.name}");
            Debug.Log($"   - Posición: {ghostController.transform.position}");
            Debug.Log($"   - Activo: {ghostController.gameObject.activeInHierarchy}");
        }
        else
        {
            Debug.LogError("❌ No se encontró GhostController");
        }
        
        // Información del enemigo
        if (enemyCommon != null)
        {
            Debug.Log($"🤖 EnemyCommon: {enemyCommon.name}");
            Debug.Log($"   - HP: {enemyCommon.CurrentHP}/{enemyCommon.MaxHP}");
            Debug.Log($"   - Muerto: {enemyCommon.IsDead}");
            Debug.Log($"   - Target: {(enemyCommon.Target != null ? "existe" : "NULL")}");
            
            if (enemyCommon.Target != null)
            {
                Debug.Log($"   - Target IsValid: {enemyCommon.Target.IsValid}");
                Debug.Log($"   - Target AimRoot: {(enemyCommon.Target.AimRoot != null ? enemyCommon.Target.AimRoot.name : "NULL")}");
                if (enemyCommon.Target.AimRoot != null)
                {
                    float dist = Vector3.Distance(ghostController.transform.position, enemyCommon.Target.AimRoot.position);
                    Debug.Log($"   - Distancia al target: {dist:F2}");
                }
            }
        }
        else
        {
            Debug.LogError("❌ No se encontró EnemyCommon");
        }
        
        // Información del ataque
        if (ghostAttack != null)
        {
            Debug.Log($"⚔️ Sistema de Ataque: {ghostAttack.GetType().Name}");
            Debug.Log($"   - CanAttack: {ghostAttack.CanAttack()}");
            
            // Solo mostrar propiedades básicas de la interfaz
            try
            {
                Debug.Log($"   - Cooldown: {ghostAttack.Cooldown:F2}s");
                Debug.Log($"   - Attack Duration: {ghostAttack.GetAttackDuration():F2}s");
            }
            catch (System.Exception e)
            {
                Debug.LogWarning($"Error obteniendo propiedades de ataque: {e.Message}");
            }
        }
        else
        {
            Debug.LogWarning("⚠️ No se encontró sistema de ataque");
        }
        
        // Información del target
        if (TargetRegistry.Instance != null)
        {
            var target = TargetRegistry.Instance.CurrentTarget;
            if (target != null)
            {
                Debug.Log($"🎯 Target: {target.GetType().Name}");
                Debug.Log($"   - IsValid: {target.IsValid}");
                Debug.Log($"   - AimRoot: {(target.AimRoot != null ? target.AimRoot.name : "NULL")}");
                if (target.AimRoot != null)
                {
                    Debug.Log($"   - Posición: {target.AimRoot.position}");
                }
            }
            else
            {
                Debug.LogWarning("⚠️ No hay target registrado");
            }
        }
        else
        {
            Debug.LogError("❌ TargetRegistry.Instance es NULL");
        }
        
        Debug.Log("=== FIN COMBAT DEBUGGER ===");
    }
    
    [ContextMenu("Debug Combat Status Now")]
    public void DebugCombatStatusNow()
    {
        DebugCombatStatus();
    }
    
    [ContextMenu("Test Attack Sequence")]
    public void TestAttackSequence()
    {
        Debug.Log("=== TESTING ATTACK SEQUENCE ===");
        
        if (ghostAttack != null)
        {
            Debug.Log("1. Verificando CanAttack()...");
            bool canAttack = ghostAttack.CanAttack();
            Debug.Log($"   Resultado: {canAttack}");
            
            if (canAttack)
            {
                Debug.Log("2. Ejecutando DoAttack()...");
                ghostAttack.DoAttack();
            }
            else
            {
                Debug.Log("2. No se puede atacar - saltando DoAttack()");
            }
        }
        else
        {
            Debug.LogError("❌ GhostAttack es NULL - no se puede probar");
        }
        
        Debug.Log("=== FIN TEST ===");
    }
}
