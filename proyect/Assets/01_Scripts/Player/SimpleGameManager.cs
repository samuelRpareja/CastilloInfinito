using UnityEngine;
using UnityEngine.SceneManagement;

public class SimpleGameManager : MonoBehaviour
{
    [Header("Configuración")]
    public float restartDelay = 3f; // tiempo antes de reiniciar
    
    private bool isDead = false;

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
                Debug.Log("✅ GameManager conectado al jugador");
            }
        }
        else
        {
            Debug.LogWarning("⚠️ No se encontró el jugador");
        }
    }

    private void Update()
    {
        if (isDead)
        {
            // Mostrar countdown en consola
            if (Time.time % 1f < Time.deltaTime)
            {
                Debug.Log("💀 Reiniciando en " + Mathf.Ceil(restartDelay - Time.time) + " segundos...");
            }
        }
    }

    private void OnPlayerDeath()
    {
        if (isDead) return;
        
        isDead = true;
        Debug.Log("💀 ¡GAME OVER! Reiniciando en " + restartDelay + " segundos...");
        
        // Reiniciar después del delay
        Invoke(nameof(RestartScene), restartDelay);
    }

    private void RestartScene()
    {
        Debug.Log("🔄 Reiniciando escena...");
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void OnDestroy()
    {
        // Limpiar suscripción
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
