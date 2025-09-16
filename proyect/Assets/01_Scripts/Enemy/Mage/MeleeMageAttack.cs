// Reemplaza tu script MeleeMageAttack.cs con este
using UnityEngine;

public class MeleeMageAttack : MonoBehaviour, IEnemyAttack
{
    [Header("Configuración del Ataque")]
    [SerializeField] private float damage = 15f;
    [SerializeField] private float range = 2.5f;
    [SerializeField] private float cooldown = 2.0f;
    [Tooltip("Duración total de la animación de ataque para bloquear el movimiento")]
    [SerializeField] private float attackDuration = 1.0f;

    [Header("Referencias")]
    [SerializeField] private Animator animator;
    // Arrastra aquí el objeto hijo que tiene el script HitboxDamager
    [SerializeField] private HitboxDamager hitbox;

    private float _lastAttackTime;
    private EnemyCommon _enemy;

    float IEnemyAttack.Cooldown => throw new System.NotImplementedException();

    private void Awake()
    {
        _enemy = GetComponent<EnemyCommon>();
        if (animator == null) animator = GetComponentInChildren<Animator>();
        // Asegúrate de que el hitbox esté desactivado al empezar
        if (hitbox != null) hitbox.gameObject.SetActive(false);
    }

    // --- Implementación Explícita de la Interfaz IEnemyAttack ---

    bool IEnemyAttack.CanAttack()
    {
        // 1. Comprobar si el cooldown ha pasado
        if (Time.time < _lastAttackTime + cooldown)
        {
            return false;
        }

        // 2. Comprobar si hay un objetivo válido
        var target = _enemy.Target;
        if (target == null || !target.IsValid)
        {
            return false;
        }

        // 3. Comprobar si el objetivo está en rango
        return Vector3.Distance(transform.position, target.AimRoot.position) <= range;
    }

    void IEnemyAttack.DoAttack()
    {
        _lastAttackTime = Time.time;
        _enemy.LockMotionFor(attackDuration);

        // Elige un ataque al azar (del 1 al 3) y lo configura en el Animator
        int randomAttackIndex = Random.Range(1, 4);
        animator.SetInteger("attackIndex", randomAttackIndex);
        animator.SetTrigger("doAttack");
    }

    float IEnemyAttack.GetAttackDuration()
    {
        return attackDuration;
    }

    // --- Funciones Públicas para Eventos de Animación ---

    // Esta función la llamarás desde el evento de animación cuando empieza el golpe
    public void OpenDamageWindow()
    {
        if (hitbox != null)
        {
            hitbox.SetDamage(damage);
            hitbox.gameObject.SetActive(true);
        }
    }

    // Esta función la llamarás desde el evento de animación cuando termina el golpe
    public void CloseDamageWindow()
    {
        if (hitbox != null)
        {
            hitbox.gameObject.SetActive(false);
        }
    }
}