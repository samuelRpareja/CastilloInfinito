using UnityEngine;

/// <summary>
/// Script que verifica que todos los sistemas estén funcionando correctamente
/// Se ejecuta al inicio y puede ser llamado manualmente
/// </summary>
public class SystemChecker : MonoBehaviour
{
    [Header("Configuración")]
    [SerializeField] private bool checkOnStart = true;
    
    void Start()
    {
        if (checkOnStart)
        {
            CheckAllSystems();
        }
    }
    
    [ContextMenu("Check All Systems")]
    public void CheckAllSystems()
    {
        Debug.Log("=== VERIFICANDO SISTEMAS ===");
        
        bool allSystemsOK = true;
        
        // 1. Verificar TargetRegistry
        if (TargetRegistry.Instance == null)
        {
            Debug.LogError("❌ TargetRegistry.Instance es NULL");
            allSystemsOK = false;
        }
        else
        {
            Debug.Log("✅ TargetRegistry existe");
        }
        
        // 2. Verificar PlayerProxy
        var playerProxy = FindObjectOfType<PlayerProxy>();
        if (playerProxy == null)
        {
            Debug.LogError("❌ No se encontró PlayerProxy en la escena");
            allSystemsOK = false;
        }
        else
        {
            Debug.Log($"✅ PlayerProxy encontrado: {playerProxy.name}");
            Debug.Log($"   - IsValid: {playerProxy.IsValid}");
            Debug.Log($"   - AimRoot: {(playerProxy.AimRoot != null ? playerProxy.AimRoot.name : "NULL")}");
            
            if (playerProxy.AimRoot == null)
            {
                Debug.LogError("❌ PlayerProxy.AimRoot es NULL");
                allSystemsOK = false;
            }
        }
        
        // 3. Verificar que el jugador esté registrado como target
        if (TargetRegistry.Instance != null)
        {
            var currentTarget = TargetRegistry.Instance.CurrentTarget;
            if (currentTarget == null)
            {
                Debug.LogError("❌ No hay target registrado en TargetRegistry");
                allSystemsOK = false;
            }
            else
            {
                Debug.Log($"✅ Target registrado: {currentTarget.GetType().Name}");
                Debug.Log($"   - IsValid: {currentTarget.IsValid}");
                Debug.Log($"   - AimRoot: {(currentTarget.AimRoot != null ? currentTarget.AimRoot.name : "NULL")}");
            }
        }
        
        // 4. Verificar GhostController
        var ghostController = FindObjectOfType<GhostController>();
        if (ghostController == null)
        {
            Debug.LogError("❌ No se encontró GhostController en la escena");
            allSystemsOK = false;
        }
        else
        {
            Debug.Log($"✅ GhostController encontrado: {ghostController.name}");
            var enemy = ghostController.GetComponent<EnemyCommon>();
            if (enemy != null)
            {
                Debug.Log($"   - EnemyCommon existe");
                Debug.Log($"   - Target: {(enemy.Target != null ? "existe" : "NULL")}");
                if (enemy.Target != null)
                {
                    Debug.Log($"   - Target IsValid: {enemy.Target.IsValid}");
                    float dist = Vector3.Distance(ghostController.transform.position, enemy.Target.AimRoot.position);
                    Debug.Log($"   - Distancia al target: {dist:F2}");
                }
            }
        }
        
        // 5. Verificar GhostMeleeAttack
        var ghostAttack = FindObjectOfType<GhostMeleeAttack>();
        if (ghostAttack == null)
        {
            Debug.LogError("❌ No se encontró GhostMeleeAttack en la escena");
            allSystemsOK = false;
        }
        else
        {
            Debug.Log($"✅ GhostMeleeAttack encontrado: {ghostAttack.name}");
            Debug.Log($"   - CanAttack: {ghostAttack.CanAttack()}");
            Debug.Log($"   - Cooldown: {ghostAttack.Cooldown}");
        }
        
        // Resumen final
        if (allSystemsOK)
        {
            Debug.Log("✅ TODOS LOS SISTEMAS ESTÁN FUNCIONANDO CORRECTAMENTE");
        }
        else
        {
            Debug.LogError("❌ HAY PROBLEMAS EN EL SISTEMA - Revisa los logs anteriores");
        }
        
        Debug.Log("=== FIN VERIFICACIÓN ===");
    }
    
    [ContextMenu("Force Register Player")]
    public void ForceRegisterPlayer()
    {
        Debug.Log("=== FORZANDO REGISTRO DEL JUGADOR ===");
        
        // Crear TargetRegistry si no existe
        if (TargetRegistry.Instance == null)
        {
            GameObject registryGO = new GameObject("TargetRegistry");
            registryGO.AddComponent<TargetRegistry>();
            Debug.Log("✅ TargetRegistry creado");
        }
        
        // Buscar jugador
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
        {
            Debug.LogError("❌ No se encontró jugador con tag 'Player'");
            return;
        }
        
        // Asegurar que tiene PlayerProxy
        var playerProxy = player.GetComponent<PlayerProxy>();
        if (playerProxy == null)
        {
            playerProxy = player.AddComponent<PlayerProxy>();
            Debug.Log("✅ PlayerProxy agregado al jugador");
        }
        
        // Forzar registro
        if (TargetRegistry.Instance != null)
        {
            TargetRegistry.Instance.Register(playerProxy);
            Debug.Log("✅ Jugador registrado como target");
        }
        
        Debug.Log("=== REGISTRO COMPLETADO ===");
    }
}
