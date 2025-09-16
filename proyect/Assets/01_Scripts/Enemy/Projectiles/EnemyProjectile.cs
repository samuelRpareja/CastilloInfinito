using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Proyectil usado por Magos y Fantasmas. Se dispara desde la boca,
/// viaja en línea recta, aplica daño a IDamageable y se destruye o
/// se devuelve al pool.
/// </summary>
[RequireComponent(typeof(Collider))]
public class EnemyProjectile : MonoBehaviour
{

    [Header("Params")]
    [SerializeField] private float defaultSpeed = 15f;
    [SerializeField] private float defaultDamage = 8f;
    [SerializeField] private float maxLifetime = 5f;
    [SerializeField] private LayerMask hitMask = ~0;

    [Header("Optional Refs")]
    [SerializeField] private Rigidbody rb;
    [SerializeField] private ParticleSystem hitVfx;

    private float _damage;
    private float _speed;
    private float _spawnTime;
    private ProjectilePool _pool;

    private void Awake()
    {
        if (rb == null) rb = GetComponent<Rigidbody>();
        var col = GetComponent<Collider>();
        col.isTrigger = true; // más barato que colisiones físicas
    }

    private void OnEnable()
    {
        _spawnTime = Time.time;
    }

    private void Update()
    {
        // Lifetime
        if (Time.time >= _spawnTime + maxLifetime)
        {
            Despawn();
        }

        // Movimiento sin rigidbody (más barato en móvil)
        if (rb == null)
        {
            transform.position += transform.forward * _speed * Time.deltaTime;
        }
    }

    private void FixedUpdate()
    {
        if (rb != null)
        {
            rb.velocity = transform.forward * _speed;
        }
    }

    public void Launch(Vector3 dir, float speed, float damage, ProjectilePool pool = null)
    {
        transform.rotation = Quaternion.LookRotation(dir);

        _speed = speed > 0 ? speed : defaultSpeed;
        _damage = damage > 0 ? damage : defaultDamage;
        _pool = pool;

        if (rb != null)
            rb.velocity = dir * _speed;
    }


    private void OnTriggerEnter(Collider other)
    {
        if (((1 << other.gameObject.layer) & hitMask.value) == 0) return;

        if (other.TryGetComponent<IDamageable>(out var dmg))
        {
            dmg.TakeDamage(_damage);

            // Affix vampírico opcional
            var vamp = GetComponentInParent<EliteVampiric>();
            if (vamp != null) vamp.OnDealDamage(_damage);
        }

        if (hitVfx != null)
        {
            Instantiate(hitVfx, transform.position, Quaternion.identity).Play();
        }

        Despawn();
    }

    private void Despawn()
    {
        if (_pool != null)
            _pool.Despawn(this);
        else
            Destroy(gameObject);
    }

    private void OnDisable()
    {
        if (rb != null) rb.velocity = Vector3.zero;
    }
}
