using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Script simple para reiniciar la escena cuando el player muere
/// Agregar este script a cualquier GameObject en la escena
/// </summary>
public class SimpleGameOver : MonoBehaviour
{
    [Header("Configuración")]
    [SerializeField] public float restartDelay = 3f;
    
    private void Start()
    {
        // Buscar PlayerProxy en la escena
        var playerProxy = FindObjectOfType<PlayerProxy>();
        
        if (playerProxy != null)
        {
            // Suscribirse al evento de muerte
            playerProxy.OnDeath += OnPlayerDeath;
            Debug.Log("SimpleGameOver: Listo para reiniciar cuando el player muera");
        }
        else
        {
            Debug.LogError("SimpleGameOver: No se encontró PlayerProxy en la escena");
        }
    }
    
    private void OnDestroy()
    {
        // Limpiar suscripción
        var playerProxy = FindObjectOfType<PlayerProxy>();
        if (playerProxy != null)
        {
            playerProxy.OnDeath -= OnPlayerDeath;
        }
    }
    
    private void OnPlayerDeath()
    {
        Debug.LogWarning("💀 PLAYER MURIÓ - Reiniciando escena en " + restartDelay + " segundos...");
        
        // Reiniciar después del delay
        Invoke(nameof(RestartScene), restartDelay);
    }
    
    private void RestartScene()
    {
        Debug.Log("🔄 Reiniciando escena...");
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
