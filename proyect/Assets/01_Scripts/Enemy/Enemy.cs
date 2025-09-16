using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Enemy Stats")]
    public float health = 100f;
    public float maxHealth = 100f;
    public float speed = 5f;
    public float damage = 10f;
    
    [Header("References")]
    public GameObject SpawnRoom;
    
    [Header("Internal")]
    protected bool isDead = false;
    
    void Start()
    {
        health = maxHealth;
    }
    
    void Update()
    {
        if (isDead) return;
        
        // Lógica básica del enemigo
        // Aquí puedes agregar la IA, movimiento, etc.
    }
    
    public virtual void TakeDamage(float damageAmount)
    {
        if (isDead) return;
        
        health -= damageAmount;
        
        if (health <= 0)
        {
            Die();
        }
    }
    
    protected virtual void Die()
    {
        isDead = true;
        
        // Notificar a la habitación que este enemigo ha muerto
        if (SpawnRoom != null)
        {
            RoomBehaveor roomBehavior = SpawnRoom.GetComponent<RoomBehaveor>();
            if (roomBehavior != null)
            {
                roomBehavior.DeleteActiveEnemy(this);
            }
        }
        
        // Aquí puedes agregar efectos de muerte, sonidos, etc.
        Destroy(gameObject);
    }
    
    public bool IsDead()
    {
        return isDead;
    }
}
