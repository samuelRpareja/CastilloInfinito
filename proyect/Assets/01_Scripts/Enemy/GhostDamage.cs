using UnityEngine;

public class GhostDamage : MonoBehaviour
{
    [Header("Configuración de Daño")]
    public float damageAmount = 5f;
    public float damageCooldown = 1f; // tiempo entre ataques
    
    private float lastDamageTime = 0f;
    private Health playerHealth;

    private void Start()
    {
        // Buscar el jugador por tag o por componente
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerHealth = player.GetComponent<Health>();
        }
        else
        {
            Debug.LogWarning("⚠️ No se encontró el jugador con tag 'Player'");
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            TryDamagePlayer();
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            TryDamagePlayer();
        }
    }

    private void TryDamagePlayer()
    {
        if (playerHealth == null || !playerHealth.IsAlive())
        {
            return;
        }

        // Verificar cooldown
        if (Time.time - lastDamageTime < damageCooldown)
        {
            return;
        }

        // Hacer daño al jugador
        playerHealth.TakeDamage(damageAmount);
        lastDamageTime = Time.time;
        
        Debug.Log($"👻 Fantasma hizo {damageAmount} de daño al jugador!");
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            TryDamagePlayer();
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            TryDamagePlayer();
        }
    }
}
