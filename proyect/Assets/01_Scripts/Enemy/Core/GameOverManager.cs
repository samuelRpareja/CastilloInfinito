using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Maneja el Game Over y reinicio de escena cuando el player muere
/// </summary>
public class GameOverManager : MonoBehaviour
{
    [Header("Configuración")]
    [SerializeField] private float restartDelay = 2f; // Tiempo antes de reiniciar
    [SerializeField] private bool showGameOverUI = true;
    
    [Header("Referencias")]
    [SerializeField] private PlayerProxy playerProxy;
    
    private bool gameOverTriggered = false;
    
    private void Start()
    {
        // Buscar PlayerProxy si no está asignado
        if (playerProxy == null)
        {
            playerProxy = FindObjectOfType<PlayerProxy>();
        }
        
        if (playerProxy != null)
        {
            // Suscribirse al evento de muerte del player
            playerProxy.OnDeath += OnPlayerDeath;
            Debug.Log("GameOverManager: Suscrito al evento de muerte del player");
        }
        else
        {
            Debug.LogError("GameOverManager: No se encontró PlayerProxy");
        }
    }
    
    private void OnDestroy()
    {
        // Desuscribirse del evento para evitar memory leaks
        if (playerProxy != null)
        {
            playerProxy.OnDeath -= OnPlayerDeath;
        }
    }
    
    private void OnPlayerDeath()
    {
        if (gameOverTriggered) return; // Evitar múltiples triggers
        
        gameOverTriggered = true;
        Debug.LogWarning("🎮 GAME OVER - Player ha muerto!");
        
        // Mostrar mensaje de Game Over
        if (showGameOverUI)
        {
            Debug.Log("💀 GAME OVER - Reiniciando escena en " + restartDelay + " segundos...");
        }
        
        // Reiniciar la escena después del delay
        Invoke(nameof(RestartScene), restartDelay);
    }
    
    private void RestartScene()
    {
        Debug.Log("🔄 Reiniciando escena...");
        
        // Obtener el nombre de la escena actual
        string currentSceneName = SceneManager.GetActiveScene().name;
        
        // Reiniciar la escena actual
        SceneManager.LoadScene(currentSceneName);
    }
    
    // Método público para reiniciar manualmente (útil para testing)
    [ContextMenu("Restart Scene Now")]
    public void RestartSceneNow()
    {
        RestartScene();
    }
    
    // Método público para simular muerte del player (útil para testing)
    [ContextMenu("Simulate Player Death")]
    public void SimulatePlayerDeath()
    {
        if (playerProxy != null)
        {
            Debug.Log("🧪 Simulando muerte del player...");
            playerProxy.TakeDamage(playerProxy.CurrentHP); // Daño suficiente para matar
        }
    }
}
