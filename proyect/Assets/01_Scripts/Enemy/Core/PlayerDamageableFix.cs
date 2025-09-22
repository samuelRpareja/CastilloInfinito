using UnityEngine;

/// <summary>
/// Script que asegura que el jugador pueda recibir daño correctamente
/// </summary>
public class PlayerDamageableFix : MonoBehaviour
{
    [Header("Configuración")]
    [SerializeField] private bool fixOnStart = true;
    [SerializeField] private float testDamage = 5f;
    
    void Start()
    {
        if (fixOnStart)
        {
            FixPlayerDamageable();
        }
    }
    
    [ContextMenu("Fix Player Damageable")]
    public void FixPlayerDamageable()
    {
        Debug.Log("=== ARREGLANDO JUGADOR PARA RECIBIR DAÑO ===");
        
        // 1. Buscar jugador
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
        {
            Debug.LogError("❌ No se encontró jugador con tag 'Player'");
            return;
        }
        
        Debug.Log($"✅ Jugador encontrado: {player.name}");
        
        // 2. Verificar/crear PlayerProxy
        PlayerProxy playerProxy = player.GetComponent<PlayerProxy>();
        if (playerProxy == null)
        {
            Debug.Log("⚠️ Agregando PlayerProxy al jugador...");
            playerProxy = player.AddComponent<PlayerProxy>();
        }
        
        Debug.Log($"✅ PlayerProxy: {playerProxy.GetType().Name}");
        Debug.Log($"   - Implementa IDamageable: {playerProxy is IDamageable}");
        Debug.Log($"   - IsValid: {playerProxy.IsValid}");
        Debug.Log($"   - AimRoot: {(playerProxy.AimRoot != null ? playerProxy.AimRoot.name : "NULL")}");
        
        // 3. Crear TargetRegistry si no existe
        if (TargetRegistry.Instance == null)
        {
            Debug.Log("⚠️ Creando TargetRegistry...");
            GameObject registryGO = new GameObject("TargetRegistry");
            registryGO.AddComponent<TargetRegistry>();
        }
        
        // 4. Registrar jugador
        if (TargetRegistry.Instance != null)
        {
            TargetRegistry.Instance.Register(playerProxy);
            Debug.Log("✅ Jugador registrado como target");
        }
        
        // 5. Verificar registro
        if (TargetRegistry.Instance != null && TargetRegistry.Instance.CurrentTarget != null)
        {
            var target = TargetRegistry.Instance.CurrentTarget;
            Debug.Log($"✅ Target registrado: {target.GetType().Name}");
            Debug.Log($"   - IsValid: {target.IsValid}");
            Debug.Log($"   - AimRoot: {(target.AimRoot != null ? target.AimRoot.name : "NULL")}");
            
            // 6. Probar daño
            Debug.Log("🧪 Probando aplicación de daño...");
            TestDamageApplication(target);
        }
        else
        {
            Debug.LogError("❌ No se pudo registrar el jugador como target");
        }
        
        Debug.Log("=== CONFIGURACIÓN COMPLETADA ===");
    }
    
    void TestDamageApplication(ITarget target)
    {
        // Buscar IDamageable
        IDamageable damageable = (target as Component)?.GetComponentInParent<IDamageable>();
        if (damageable == null)
        {
            Debug.LogError("❌ Target no implementa IDamageable");
            
            // Buscar en el GameObject del target
            var targetGO = target.AimRoot.gameObject;
            var allComponents = targetGO.GetComponents<Component>();
            Debug.Log($"Componentes en {targetGO.name}:");
            foreach (var comp in allComponents)
            {
                Debug.Log($"  - {comp.GetType().Name}");
                if (comp is IDamageable)
                {
                    Debug.Log($"    ✅ ¡Este componente implementa IDamageable!");
                }
            }
            return;
        }
        
        Debug.Log($"✅ IDamageable encontrado: {damageable.GetType().Name}");
        
        // Aplicar daño de prueba
        Debug.Log($"🧪 Aplicando {testDamage} de daño de prueba...");
        damageable.TakeDamage(testDamage);
        Debug.Log("✅ ¡Daño aplicado correctamente!");
    }
    
    [ContextMenu("Test Damage Now")]
    public void TestDamageNow()
    {
        if (TargetRegistry.Instance == null || TargetRegistry.Instance.CurrentTarget == null)
        {
            Debug.LogError("❌ No hay target registrado");
            return;
        }
        
        TestDamageApplication(TargetRegistry.Instance.CurrentTarget);
    }
}
