// Crea este script y ponlo en el objeto Hitbox del arma/mano del mago
using UnityEngine;

public class HitboxDamager : MonoBehaviour
{
    private float _damage;

    // M�todo para que el script de ataque configure el da�o
    public void SetDamage(float damage)
    {
        _damage = damage;
    }

    // Esto se ejecuta cuando el trigger del hitbox entra en contacto con otro collider
    private void OnTriggerEnter(Collider other)
    {
        // Buscamos un componente que pueda recibir da�o (como el del jugador o un dummy)
        IDamageable damageableTarget = other.GetComponent<IDamageable>();

        if (damageableTarget != null)
        {
            // Si lo encontramos, le aplicamos el da�o
            Debug.Log($"[HitboxDamager] Golpe� a {other.name}! Aplicando {_damage} de da�o.");
            damageableTarget.TakeDamage(_damage);

            // Opcional: Desactivar el hitbox despu�s de un golpe para evitar da�o m�ltiple
            // gameObject.SetActive(false); 
        }
    }
}