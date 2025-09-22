using UnityEngine;

/// <summary>
/// Script simple para probar el combate sin errores
/// </summary>
public class SimpleCombatTest : MonoBehaviour
{
    [Header("Configuración")]
    [SerializeField] private bool enableTest = true;
    [SerializeField] private float testInterval = 2f;
    
    private float lastTestTime;
    
    void Update()
    {
        if (!enableTest) return;
        
        if (Time.time - lastTestTime >= testInterval)
        {
            TestCombatSystem();
            lastTestTime = Time.time;
        }
    }
    
    void TestCombatSystem()
    {
        Debug.Log("=== SIMPLE COMBAT TEST ===");
        
        // 1. Verificar TargetRegistry
        if (TargetRegistry.Instance == null)
        {
            Debug.LogError("❌ TargetRegistry.Instance es NULL");
            return;
        }
        Debug.Log("✅ TargetRegistry existe");
        
        // 2. Verificar target
        var target = TargetRegistry.Instance.CurrentTarget;
        if (target == null)
        {
            Debug.LogWarning("⚠️ No hay target registrado");
            return;
        }
        Debug.Log($"✅ Target existe: {target.GetType().Name}");
        Debug.Log($"   - IsValid: {target.IsValid}");
        
        // 3. Verificar fantasma
        var ghostController = FindObjectOfType<GhostController>();
        if (ghostController == null)
        {
            Debug.LogError("❌ No se encontró GhostController");
            return;
        }
        Debug.Log($"✅ GhostController encontrado: {ghostController.name}");
        
        // 4. Verificar sistema de ataque
        var attack = ghostController.GetComponent<IEnemyAttack>();
        if (attack == null)
        {
            Debug.LogWarning("⚠️ No se encontró sistema de ataque");
            return;
        }
        Debug.Log($"✅ Sistema de ataque: {attack.GetType().Name}");
        Debug.Log($"   - CanAttack: {attack.CanAttack()}");
        
        // 5. Verificar distancia
        if (target.IsValid && target.AimRoot != null)
        {
            float distance = Vector3.Distance(ghostController.transform.position, target.AimRoot.position);
            Debug.Log($"📏 Distancia: {distance:F2}");
        }
        
        Debug.Log("=== FIN TEST ===");
    }
    
    [ContextMenu("Test Now")]
    public void TestNow()
    {
        TestCombatSystem();
    }
}
