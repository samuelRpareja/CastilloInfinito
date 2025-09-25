using UnityEngine;
using UnityEngine.UI;

public class HealthUI : MonoBehaviour
{
    [Header("UI Elements")]
    public Slider healthBar;
    public Text healthText;
    public Image healthBarFill;
    
    [Header("Colors")]
    public Color healthyColor = Color.green;
    public Color lowHealthColor = Color.red;
    public Color mediumHealthColor = Color.yellow;

    private Health playerHealth;
    private float lastDamageAmount = 0f;

    private void Start()
    {
        // Buscar el jugador
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerHealth = player.GetComponent<Health>();
            if (playerHealth != null)
            {
                playerHealth.OnHealthChanged += UpdateHealthUI;
                playerHealth.OnDamageTaken += OnPlayerDamageTaken;
                UpdateHealthUI(playerHealth.currentHP);
            }
        }
        else
        {
            Debug.LogWarning("⚠️ No se encontró el jugador para mostrar la vida");
        }
    }

    private void OnDestroy()
    {
        if (playerHealth != null)
        {
            playerHealth.OnHealthChanged -= UpdateHealthUI;
            playerHealth.OnDamageTaken -= OnPlayerDamageTaken;
        }
    }

    private void UpdateHealthUI(float currentHealth)
    {
        if (playerHealth == null) return;

        float healthPercentage = playerHealth.GetHealthPercentage();
        
        // Actualizar barra de vida
        if (healthBar != null)
        {
            healthBar.value = healthPercentage;
        }

        // Actualizar texto de vida con formato similar a TargetDummy
        if (healthText != null)
        {
            if (lastDamageAmount > 0f)
            {
                healthText.text = $"[Player] Daño {lastDamageAmount:F0} → HP: {currentHealth:F0}/{playerHealth.maxHP:F0}";
                lastDamageAmount = 0f; // Resetear después de mostrar
            }
            else
            {
                healthText.text = $"[Player] HP: {currentHealth:F0}/{playerHealth.maxHP:F0}";
            }
        }

        // Cambiar color según la vida
        if (healthBarFill != null)
        {
            if (healthPercentage > 0.6f)
            {
                healthBarFill.color = healthyColor;
            }
            else if (healthPercentage > 0.3f)
            {
                healthBarFill.color = mediumHealthColor;
            }
            else
            {
                healthBarFill.color = lowHealthColor;
            }
        }
    }
    
    private void OnPlayerDamageTaken(float currentHP, float damageAmount)
    {
        lastDamageAmount = damageAmount;
        UpdateHealthUI(currentHP);
    }
}
