using UnityEngine;

public class Health : MonoBehaviour
{
    [Header("Configuración de Salud")]
    public float maxHP = 10f;
    public float currentHP = 10f;
    public float damageReduction = 0f; // porcentaje de reducción (0 a 1)

    [Header("Eventos")]
    public System.Action<float> OnHealthChanged;
    public System.Action OnDeath;

    private void Start()
    {
        currentHP = maxHP;
        OnHealthChanged?.Invoke(currentHP);
    }

    public void TakeDamage(float amount)
    {
        float dmg = amount * (1f - damageReduction);
        currentHP -= dmg;
        
        // Asegurar que no baje de 0
        if (currentHP < 0f) currentHP = 0f;
        
        OnHealthChanged?.Invoke(currentHP);
        
        Debug.Log($"💔 {gameObject.name} recibió {dmg} de daño. Vida restante: {currentHP}/{maxHP}");
        
        if (currentHP <= 0f) 
        {
            Die();
        }
    }

    public void Heal(float amount)
    {
        currentHP += amount;
        
        // Asegurar que no suba del máximo
        if (currentHP > maxHP) currentHP = maxHP;
        
        OnHealthChanged?.Invoke(currentHP);
        
        Debug.Log($"💚 {gameObject.name} se curó {amount}. Vida actual: {currentHP}/{maxHP}");
    }

    public void Die()
    {
        Debug.Log($"💀 {gameObject.name} ha muerto!");
        OnDeath?.Invoke();
        
        // No destruir inmediatamente, dejar que GameManager maneje el reinicio
        // Destroy(gameObject);
    }

    public float GetHealthPercentage()
    {
        return currentHP / maxHP;
    }

    public bool IsAlive()
    {
        return currentHP > 0f;
    }
}
