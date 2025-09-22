using UnityEngine;

/// <summary>
/// Script simple para debug rápido del sistema de combate
/// </summary>
public class QuickDebug : MonoBehaviour
{
    void Start()
    {
        Debug.Log("=== QUICK DEBUG INICIANDO ===");
        
        // Verificar TargetRegistry
        if (TargetRegistry.Instance == null)
        {
            Debug.LogError("❌ TargetRegistry.Instance es NULL");
        }
        else
        {
            Debug.Log("✅ TargetRegistry existe");
        }
        
        // Verificar jugador
        var playerProxy = FindObjectOfType<PlayerProxy>();
        if (playerProxy == null)
        {
            Debug.LogError("❌ No se encontró PlayerProxy");
        }
        else
        {
            Debug.Log($"✅ PlayerProxy encontrado: {playerProxy.name}");
            Debug.Log($"   - IsValid: {playerProxy.IsValid}");
            Debug.Log($"   - AimRoot: {(playerProxy.AimRoot != null ? playerProxy.AimRoot.name : "NULL")}");
        }
        
        // Verificar fantasma
        var ghostController = FindObjectOfType<GhostController>();
        if (ghostController == null)
        {
            Debug.LogError("❌ No se encontró GhostController");
        }
        else
        {
            Debug.Log($"✅ GhostController encontrado: {ghostController.name}");
            
            var ghostAttack = ghostController.GetComponent<GhostMeleeAttack>();
            if (ghostAttack == null)
            {
                Debug.LogError("❌ No se encontró GhostMeleeAttack");
            }
            else
            {
                Debug.Log($"✅ GhostMeleeAttack encontrado: {ghostAttack.name}");
                Debug.Log($"   - Rango: {ghostAttack.Range}");
                Debug.Log($"   - Daño: {ghostAttack.Damage}");
                Debug.Log($"   - Cooldown: {ghostAttack.Cooldown}");
            }
        }
        
        Debug.Log("=== QUICK DEBUG COMPLETADO ===");
    }
}
