using UnityEngine;

public class DebugTargetSystem : MonoBehaviour
{
    [Header("Debug Settings")]
    [SerializeField] private bool enableDebug = true;
    [SerializeField] private float debugInterval = 2f;
    
    private float lastDebugTime;
    
    private void Update()
    {
        if (!enableDebug) return;
        
        if (Time.time - lastDebugTime >= debugInterval)
        {
            DebugTargetStatus();
            lastDebugTime = Time.time;
        }
    }
    
    private void DebugTargetStatus()
    {
        Debug.Log("=== DEBUG TARGET SYSTEM ===");
        
        // Verificar TargetRegistry
        if (TargetRegistry.Instance == null)
        {
            Debug.LogError("❌ TargetRegistry.Instance es NULL");
            return;
        }
        else
        {
            Debug.Log("✅ TargetRegistry.Instance existe");
        }
        
        // Verificar CurrentTarget
        var currentTarget = TargetRegistry.Instance.CurrentTarget;
        if (currentTarget == null)
        {
            Debug.LogWarning("⚠️ CurrentTarget es NULL - El jugador no está registrado");
        }
        else
        {
            Debug.Log($"✅ CurrentTarget existe: {currentTarget.GetType().Name}");
            Debug.Log($"   - IsValid: {currentTarget.IsValid}");
            Debug.Log($"   - AimRoot: {(currentTarget.AimRoot != null ? currentTarget.AimRoot.name : "NULL")}");
            if (currentTarget.AimRoot != null)
            {
                Debug.Log($"   - Posición: {currentTarget.AimRoot.position}");
            }
        }
        
        // Verificar PlayerProxy
        var playerProxy = FindObjectOfType<PlayerProxy>();
        if (playerProxy == null)
        {
            Debug.LogError("❌ No se encontró PlayerProxy en la escena");
        }
        else
        {
            Debug.Log($"✅ PlayerProxy encontrado: {playerProxy.name}");
            Debug.Log($"   - IsValid: {playerProxy.IsValid}");
            Debug.Log($"   - IsDead: {playerProxy.IsDead}");
            Debug.Log($"   - Active: {playerProxy.gameObject.activeInHierarchy}");
            Debug.Log($"   - Enabled: {playerProxy.enabled}");
        }
        
        // Verificar GhostController
        var ghostController = FindObjectOfType<GhostController>();
        if (ghostController == null)
        {
            Debug.LogError("❌ No se encontró GhostController en la escena");
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
                    Debug.Log($"   - Distancia al target: {Vector3.Distance(ghostController.transform.position, enemy.Target.AimRoot.position):F2}");
                }
            }
        }
        
        Debug.Log("=== FIN DEBUG ===");
    }
    
    [ContextMenu("Debug Target System Now")]
    public void DebugTargetSystemNow()
    {
        DebugTargetStatus();
    }
}
