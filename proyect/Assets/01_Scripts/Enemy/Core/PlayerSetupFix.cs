using UnityEngine;

/// <summary>
/// Script para arreglar la configuración del jugador y asegurar que tenga IDamageable
/// </summary>
public class PlayerSetupFix : MonoBehaviour
{
    [Header("Configuración")]
    [SerializeField] private bool fixOnStart = true;
    [SerializeField] private bool showDebugLogs = true;
    
    void Start()
    {
        if (fixOnStart)
        {
            FixPlayerSetup();
        }
    }
    
    [ContextMenu("Fix Player Setup")]
    public void FixPlayerSetup()
    {
        Debug.Log("=== ARREGLANDO CONFIGURACIÓN DEL JUGADOR ===");
        
        // 1. Buscar el jugador
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
        {
            Debug.LogError("❌ No se encontró jugador con tag 'Player'");
            return;
        }
        
        Debug.Log($"✅ Jugador encontrado: {player.name}");
        
        // 2. Asegurar que tiene PlayerProxy
        PlayerProxy playerProxy = player.GetComponent<PlayerProxy>();
        if (playerProxy == null)
        {
            playerProxy = player.AddComponent<PlayerProxy>();
            Debug.Log("✅ PlayerProxy agregado al jugador");
        }
        else
        {
            Debug.Log("✅ PlayerProxy ya existe en el jugador");
        }
        
        // 3. Verificar que PlayerProxy implementa IDamageable
        if (playerProxy is IDamageable)
        {
            Debug.Log("✅ PlayerProxy implementa IDamageable");
        }
        else
        {
            Debug.LogError("❌ PlayerProxy NO implementa IDamageable");
        }
        
        // 4. Asegurar que existe TargetRegistry
        if (TargetRegistry.Instance == null)
        {
            GameObject registryGO = new GameObject("TargetRegistry");
            registryGO.AddComponent<TargetRegistry>();
            Debug.Log("✅ TargetRegistry creado");
        }
        else
        {
            Debug.Log("✅ TargetRegistry ya existe");
        }
        
        // 5. Registrar el jugador como target
        if (TargetRegistry.Instance != null)
        {
            TargetRegistry.Instance.Register(playerProxy);
            Debug.Log("✅ Jugador registrado como target");
        }
        
        // 6. Verificar que el jugador está registrado
        if (TargetRegistry.Instance != null && TargetRegistry.Instance.CurrentTarget != null)
        {
            Debug.Log($"✅ Target registrado: {TargetRegistry.Instance.CurrentTarget.GetType().Name}");
            Debug.Log($"   - IsValid: {TargetRegistry.Instance.CurrentTarget.IsValid}");
            Debug.Log($"   - AimRoot: {(TargetRegistry.Instance.CurrentTarget.AimRoot != null ? TargetRegistry.Instance.CurrentTarget.AimRoot.name : "NULL")}");
        }
        else
        {
            Debug.LogError("❌ No hay target registrado");
        }
        
        Debug.Log("=== CONFIGURACIÓN COMPLETADA ===");
    }
    
    [ContextMenu("Test Damage Application")]
    public void TestDamageApplication()
    {
        Debug.Log("=== PROBANDO APLICACIÓN DE DAÑO ===");
        
        if (TargetRegistry.Instance == null)
        {
            Debug.LogError("❌ TargetRegistry.Instance es NULL");
            return;
        }
        
        var target = TargetRegistry.Instance.CurrentTarget;
        if (target == null)
        {
            Debug.LogError("❌ No hay target registrado");
            return;
        }
        
        Debug.Log($"✅ Target encontrado: {target.GetType().Name}");
        Debug.Log($"   - IsValid: {target.IsValid}");
        Debug.Log($"   - AimRoot: {(target.AimRoot != null ? target.AimRoot.name : "NULL")}");
        
        // Buscar IDamageable
        IDamageable damageable = (target as Component)?.GetComponentInParent<IDamageable>();
        if (damageable == null)
        {
            Debug.LogError("❌ Target no tiene componente IDamageable");
            
            // Buscar en todos los componentes
            var components = target.AimRoot.GetComponents<Component>();
            Debug.Log($"📋 Componentes encontrados en {target.AimRoot.name}:");
            foreach (var comp in components)
            {
                Debug.Log($"   - {comp.GetType().Name}");
            }
        }
        else
        {
            Debug.Log($"✅ IDamageable encontrado: {damageable.GetType().Name}");
            Debug.Log("✅ Aplicando 5 de daño de prueba...");
            damageable.TakeDamage(5f);
            Debug.Log("✅ Daño aplicado correctamente");
        }
        
        Debug.Log("=== PRUEBA COMPLETADA ===");
    }
}
