using UnityEngine;

/// <summary>
/// Script que configura automáticamente el sistema de Game Over
/// Agregar este script a cualquier GameObject en la escena para configuración automática
/// </summary>
public class AutoGameOverSetup : MonoBehaviour
{
    [Header("Configuración")]
    [SerializeField] private float restartDelay = 3f;
    [SerializeField] private bool setupOnStart = true;
    
    private void Start()
    {
        if (setupOnStart)
        {
            SetupGameOver();
        }
    }
    
    [ContextMenu("Setup Game Over")]
    public void SetupGameOver()
    {
        Debug.Log("🔧 Configurando sistema de Game Over automáticamente...");
        
        // Verificar si ya existe un SimpleGameOver
        var existingGameOver = FindObjectOfType<SimpleGameOver>();
        if (existingGameOver != null)
        {
            Debug.Log("✅ Ya existe un SimpleGameOver en la escena");
            return;
        }
        
        // Crear un GameObject para el Game Over Manager
        GameObject gameOverObject = new GameObject("GameOverManager");
        gameOverObject.transform.SetParent(transform);
        
        // Agregar el componente SimpleGameOver
        var gameOver = gameOverObject.AddComponent<SimpleGameOver>();
        
        // Configurar el delay
        gameOver.restartDelay = restartDelay;
        
        Debug.Log($"✅ Sistema de Game Over configurado con delay de {restartDelay} segundos");
        Debug.Log("💡 El juego se reiniciará automáticamente cuando el player muera");
    }
    
    // Método para testing - simular muerte del player
    [ContextMenu("Test Player Death")]
    public void TestPlayerDeath()
    {
        var playerProxy = FindObjectOfType<PlayerProxy>();
        if (playerProxy != null)
        {
            Debug.Log("🧪 Probando muerte del player...");
            playerProxy.TakeDamage(playerProxy.CurrentHP);
        }
        else
        {
            Debug.LogError("❌ No se encontró PlayerProxy para testing");
        }
    }
}
