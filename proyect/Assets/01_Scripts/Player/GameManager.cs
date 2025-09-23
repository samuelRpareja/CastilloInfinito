using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [Header("Configuración de Reinicio")]
    public float restartDelay = 2f; // tiempo antes de reiniciar (en segundos)
    
    [Header("UI de Muerte")]
    public GameObject deathUI; // UI que aparece cuando mueres (opcional)
    
    private bool isGameOver = false;
    private float restartTimer = 0f;

    private void Start()
    {
        // Buscar el jugador y suscribirse a su muerte
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            Health playerHealth = player.GetComponent<Health>();
            if (playerHealth != null)
            {
                playerHealth.OnDeath += OnPlayerDeath;
            }
        }
        else
        {
            Debug.LogWarning("⚠️ No se encontró el jugador para suscribirse a su muerte");
        }
    }

    private void Update()
    {
        // Si el juego terminó, contar hacia el reinicio
        if (isGameOver)
        {
            restartTimer += Time.deltaTime;
            
            // Mostrar tiempo restante en consola
            if (restartTimer % 1f < Time.deltaTime) // Cada segundo
            {
                float timeLeft = restartDelay - restartTimer;
                if (timeLeft > 0)
                {
                    Debug.Log($"💀 Reiniciando en {timeLeft:F0} segundos...");
                }
            }
            
            // Reiniciar cuando se acabe el tiempo
            if (restartTimer >= restartDelay)
            {
                RestartScene();
            }
        }
    }

    private void OnPlayerDeath()
    {
        if (isGameOver) return; // Evitar múltiples llamadas
        
        isGameOver = true;
        restartTimer = 0f;
        
        Debug.Log("💀 ¡GAME OVER! La escena se reiniciará en " + restartDelay + " segundos...");
        
        // Mostrar UI de muerte si está configurada
        if (deathUI != null)
        {
            deathUI.SetActive(true);
        }
        
        // Pausar el juego (opcional)
        Time.timeScale = 0.5f; // Ralentizar el tiempo
    }

    public void RestartScene()
    {
        Debug.Log("🔄 Reiniciando escena...");
        Time.timeScale = 1f; // Restaurar velocidad normal
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void RestartSceneImmediately()
    {
        Debug.Log("🔄 Reiniciando escena inmediatamente...");
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void OnDestroy()
    {
        // Desuscribirse para evitar errores
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            Health playerHealth = player.GetComponent<Health>();
            if (playerHealth != null)
            {
                playerHealth.OnDeath -= OnPlayerDeath;
            }
        }
    }
}
