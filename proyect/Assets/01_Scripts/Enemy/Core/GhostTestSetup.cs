using UnityEngine;

/// <summary>
/// Script de prueba para configurar rápidamente el sistema de fantasmas
/// Coloca este script en un GameObject vacío en la escena
/// </summary>
public class GhostTestSetup : MonoBehaviour
{
    [Header("Configuración de Prueba")]
    [SerializeField] private GameObject playerPrefab; // Asigna tu prefab del jugador
    [SerializeField] private GameObject ghostPrefab;  // Asigna tu prefab del fantasma
    [SerializeField] private Vector3 playerPosition = new Vector3(0, 0, 0);
    [SerializeField] private Vector3 ghostPosition = new Vector3(5, 0, 0);
    
    [ContextMenu("Setup Test Scene")]
    public void SetupTestScene()
    {
        Debug.Log("=== CONFIGURANDO ESCENA DE PRUEBA ===");
        
        // 1. Crear TargetRegistry
        if (TargetRegistry.Instance == null)
        {
            GameObject registryGO = new GameObject("TargetRegistry");
            registryGO.AddComponent<TargetRegistry>();
            Debug.Log("✓ TargetRegistry creado");
        }
        
        // 2. Crear o configurar jugador
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null && playerPrefab != null)
        {
            player = Instantiate(playerPrefab, playerPosition, Quaternion.identity);
            player.name = "TestPlayer";
            Debug.Log("✓ Jugador de prueba creado");
        }
        
        if (player != null)
        {
            // Asegurar que tiene PlayerTargetSetup
            if (player.GetComponent<PlayerTargetSetup>() == null)
            {
                player.AddComponent<PlayerTargetSetup>();
                Debug.Log("✓ PlayerTargetSetup agregado al jugador");
            }
            
            // Asegurar que tiene PlayerProxy
            if (player.GetComponent<PlayerProxy>() == null)
            {
                player.AddComponent<PlayerProxy>();
                Debug.Log("✓ PlayerProxy agregado al jugador");
            }
        }
        
        // 3. Crear fantasma de prueba
        if (ghostPrefab != null)
        {
            GameObject ghost = Instantiate(ghostPrefab, ghostPosition, Quaternion.identity);
            ghost.name = "TestGhost";
            Debug.Log("✓ Fantasma de prueba creado");
        }
        
        Debug.Log("=== CONFIGURACIÓN COMPLETADA ===");
        Debug.Log("Ahora mueve el jugador cerca del fantasma para probar la persecución");
    }
    
    void OnDrawGizmos()
    {
        // Dibujar posiciones de spawn
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(playerPosition, 0.5f);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(ghostPosition, 0.5f);
        
        // Dibujar línea entre jugador y fantasma
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(playerPosition, ghostPosition);
    }
}
