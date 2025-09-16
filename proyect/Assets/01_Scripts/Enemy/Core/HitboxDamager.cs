// Crea este script y ponlo en el objeto Hitbox del arma/mano del mago
using UnityEngine;

public class HitboxDamager : MonoBehaviour
{
    private float _damage;

    // Método para que el script de ataque configure el daño
    public void SetDamage(float damage)
    {
        _damage = damage;
    }

    // Esto se ejecuta cuando el trigger del hitbox entra en contacto con otro collider
    private void OnTriggerEnter(Collider other)
    {
        // Buscamos un componente que pueda recibir daño (como el del jugador o un dummy)
        IDamageable damageableTarget = other.GetComponent<IDamageable>();

        if (damageableTarget != null)
        {
            // Si lo encontramos, le aplicamos el daño
            Debug.Log($"[HitboxDamager] Golpeó a {other.name}! Aplicando {_damage} de daño.");
            damageableTarget.TakeDamage(_damage);

            // Opcional: Desactivar el hitbox después de un golpe para evitar daño múltiple
            // gameObject.SetActive(false); 
        }
    }
}