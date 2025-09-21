using UnityEngine;

/// <summary>
/// Script que inicializa todos los sistemas necesarios para el juego
/// Debe estar en un GameObject en la escena
/// </summary>
public class GameInitializer : MonoBehaviour
{
    [Header("Configuración")]
    [SerializeField] private bool initializeOnStart = true;
    [SerializeField] private bool showDebugLogs = true;
    
    void Start()
    {
        if (initializeOnStart)
        {
            InitializeGameSystems();
        }
    }
    
    [ContextMenu("Initialize Game Systems")]
    public void InitializeGameSystems()
    {
        Debug.Log("=== INICIALIZANDO SISTEMAS DEL JUEGO ===");
        
        // 1. Crear TargetRegistry si no existe
        if (TargetRegistry.Instance == null)
        {
            GameObject registryGO = new GameObject("TargetRegistry");
            registryGO.AddComponent<TargetRegistry>();
            Debug.Log("✓ TargetRegistry creado");
        }
        else
        {
            Debug.Log("✓ TargetRegistry ya existe");
        }
        
        // 2. Buscar jugador y configurarlo como target
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            Debug.Log($"✓ Jugador encontrado: {player.name}");
            
            // Agregar PlayerTargetSetup si no lo tiene
            PlayerTargetSetup playerSetup = player.GetComponent<PlayerTargetSetup>();
            if (playerSetup == null)
            {
                playerSetup = player.AddComponent<PlayerTargetSetup>();
                Debug.Log("✓ PlayerTargetSetup agregado al jugador");
            }
            else
            {
                Debug.Log("✓ PlayerTargetSetup ya existe en el jugador");
            }
            
            // Verificar que el jugador esté registrado como target
            if (TargetRegistry.Instance.CurrentTarget != null)
            {
                Debug.Log($"✓ Target registrado: {TargetRegistry.Instance.CurrentTarget}");
            }
            else
            {
                Debug.LogWarning("⚠ Target no está registrado. El jugador debería registrarse automáticamente.");
            }
        }
        else
        {
            Debug.LogError("✗ No se encontró jugador con tag 'Player'");
        }
        
        // 3. Buscar fantasmas en la escena
        GhostController[] ghosts = FindObjectsOfType<GhostController>();
        Debug.Log($"✓ Encontrados {ghosts.Length} fantasmas en la escena");
        
        foreach (var ghost in ghosts)
        {
            if (showDebugLogs)
            {
                Debug.Log($"  - Fantasma: {ghost.name} (Inicializado: {ghost.GetComponent<EnemyCommon>()?.enabled})");
            }
        }
        
        Debug.Log("=== INICIALIZACIÓN COMPLETADA ===");
    }
    
    void OnDrawGizmos()
    {
        // Dibujar esfera de detección para fantasmas
        GhostController[] ghosts = FindObjectsOfType<GhostController>();
        foreach (var ghost in ghosts)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(ghost.transform.position, 12f); // aggroRange
        }
    }
}
