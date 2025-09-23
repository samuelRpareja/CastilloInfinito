using UnityEngine;

/// <summary>
/// Script para limpiar el sistema de ataque problemático y restaurar el movimiento normal
/// </summary>
public class PlayerAttackCleanup : MonoBehaviour
{
    [ContextMenu("Limpiar Sistema de Ataque Problemático")]
    public void CleanupProblematicAttackSystem()
    {
        Debug.Log("Iniciando limpieza del sistema de ataque problemático...");
        
        // 1. Remover PlayerAttackController si existe
        PlayerAttackController attackController = GetComponent<PlayerAttackController>();
        if (attackController != null)
        {
            DestroyImmediate(attackController);
            Debug.Log("Removido PlayerAttackController");
        }
        
        // 2. Remover PlayerAttackDamage si existe
        PlayerAttackDamage attackDamage = GetComponent<PlayerAttackDamage>();
        if (attackDamage != null)
        {
            DestroyImmediate(attackDamage);
            Debug.Log("Removido PlayerAttackDamage");
        }
        
        // 3. Remover PlayerAttackSetup si existe
        PlayerAttackSetup attackSetup = GetComponent<PlayerAttackSetup>();
        if (attackSetup != null)
        {
            DestroyImmediate(attackSetup);
            Debug.Log("Removido PlayerAttackSetup");
        }
        
        // 4. Buscar y remover hitbox problemático
        Transform hitboxTransform = transform.Find("AttackHitbox");
        if (hitboxTransform != null)
        {
            DestroyImmediate(hitboxTransform.gameObject);
            Debug.Log("Removido AttackHitbox problemático");
        }
        
        // 5. Buscar y remover cualquier PlayerHitbox en hijos
        PlayerHitbox[] hitboxes = GetComponentsInChildren<PlayerHitbox>();
        foreach (PlayerHitbox hitbox in hitboxes)
        {
            if (hitbox != null)
            {
                DestroyImmediate(hitbox.gameObject);
                Debug.Log("Removido PlayerHitbox: " + hitbox.name);
            }
        }
        
        Debug.Log("Limpieza completada. El personaje debería moverse normalmente ahora.");
    }
    
    [ContextMenu("Configurar Sistema de Ataque Simple")]
    public void SetupSimpleAttackSystem()
    {
        Debug.Log("Configurando sistema de ataque simple...");
        
        // Limpiar primero
        CleanupProblematicAttackSystem();
        
        // Agregar SimplePlayerAttack
        SimplePlayerAttack simpleAttack = GetComponent<SimplePlayerAttack>();
        if (simpleAttack == null)
        {
            simpleAttack = gameObject.AddComponent<SimplePlayerAttack>();
            Debug.Log("Agregado SimplePlayerAttack");
        }
        
        // Configurar valores por defecto
        var damageField = typeof(SimplePlayerAttack).GetField("attackDamage", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        damageField?.SetValue(simpleAttack, 10f);
        
        var rangeField = typeof(SimplePlayerAttack).GetField("attackRange", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        rangeField?.SetValue(simpleAttack, 2f);
        
        var angleField = typeof(SimplePlayerAttack).GetField("attackAngle", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        angleField?.SetValue(simpleAttack, 120f);
        
        Debug.Log("Sistema de ataque simple configurado correctamente");
    }
}
