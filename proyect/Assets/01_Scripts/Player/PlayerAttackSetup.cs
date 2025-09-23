using UnityEngine;

/// <summary>
/// Script de configuración automática para el sistema de ataque del personaje
/// </summary>
public class PlayerAttackSetup : MonoBehaviour
{
    [Header("Configuración del Hitbox")]
    [SerializeField] private float hitboxSize = 1.5f;
    [SerializeField] private Vector3 hitboxOffset = new Vector3(0, 0, 0.5f);
    [SerializeField] private float attackDamage = 10f;
    
    [ContextMenu("Configurar Sistema de Ataque")]
    public void SetupAttackSystem()
    {
        Debug.Log("Iniciando configuración del sistema de ataque...");
        
        // 1. Agregar PlayerAttackController si no existe
        PlayerAttackController attackController = GetComponent<PlayerAttackController>();
        if (attackController == null)
        {
            attackController = gameObject.AddComponent<PlayerAttackController>();
            Debug.Log("Agregado PlayerAttackController");
        }
        
        // 2. Crear GameObject para el hitbox
        GameObject hitboxObj = transform.Find("AttackHitbox")?.gameObject;
        if (hitboxObj == null)
        {
            hitboxObj = new GameObject("AttackHitbox");
            hitboxObj.transform.SetParent(transform);
            hitboxObj.transform.localPosition = hitboxOffset;
            Debug.Log("Creado GameObject AttackHitbox");
        }
        
        // 3. Agregar Collider al hitbox
        Collider hitboxCollider = hitboxObj.GetComponent<Collider>();
        if (hitboxCollider == null)
        {
            hitboxCollider = hitboxObj.AddComponent<BoxCollider>();
            Debug.Log("Agregado BoxCollider al hitbox");
        }
        
        // Configurar como trigger
        hitboxCollider.isTrigger = true;
        
        // Configurar tamaño
        if (hitboxCollider is BoxCollider boxCollider)
        {
            boxCollider.size = Vector3.one * hitboxSize;
        }
        
        // 4. Agregar PlayerHitbox al hitbox
        PlayerHitbox playerHitbox = hitboxObj.GetComponent<PlayerHitbox>();
        if (playerHitbox == null)
        {
            playerHitbox = hitboxObj.AddComponent<PlayerHitbox>();
            Debug.Log("Agregado PlayerHitbox");
        }
        
        // 5. Configurar el PlayerAttackController
        var attackControllerField = typeof(PlayerAttackController).GetField("hitbox", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        attackControllerField?.SetValue(attackController, playerHitbox);
        
        var damageField = typeof(PlayerAttackController).GetField("attackDamage", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        damageField?.SetValue(attackController, attackDamage);
        
        // 6. Configurar el PlayerHitbox
        var hitboxDamageField = typeof(PlayerHitbox).GetField("damageAmount", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        hitboxDamageField?.SetValue(playerHitbox, attackDamage);
        
        // 7. Desactivar el hitbox inicialmente
        hitboxObj.SetActive(false);
        
        Debug.Log("Configuración del sistema de ataque completada");
        Debug.Log($"- Hitbox creado en: {hitboxObj.name}");
        Debug.Log($"- Daño configurado: {attackDamage}");
        Debug.Log($"- Tamaño del hitbox: {hitboxSize}");
    }
    
    [ContextMenu("Limpiar Sistema de Ataque")]
    public void CleanupAttackSystem()
    {
        Debug.Log("Limpiando sistema de ataque...");
        
        // Remover PlayerAttackController
        PlayerAttackController attackController = GetComponent<PlayerAttackController>();
        if (attackController != null)
        {
            DestroyImmediate(attackController);
            Debug.Log("Removido PlayerAttackController");
        }
        
        // Remover hitbox
        Transform hitboxTransform = transform.Find("AttackHitbox");
        if (hitboxTransform != null)
        {
            DestroyImmediate(hitboxTransform.gameObject);
            Debug.Log("Removido AttackHitbox");
        }
        
        Debug.Log("Limpieza completada");
    }
    
    void OnDrawGizmos()
    {
        // Dibujar preview del hitbox
        Gizmos.color = new Color(1f, 0f, 0f, 0.3f);
        Vector3 hitboxPosition = transform.position + hitboxOffset;
        Gizmos.DrawCube(hitboxPosition, Vector3.one * hitboxSize);
        
        // Dibujar outline
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(hitboxPosition, Vector3.one * hitboxSize);
    }
}
