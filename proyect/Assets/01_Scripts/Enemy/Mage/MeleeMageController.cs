// Pon este script en Assets/01_Scripts/Enemy/Mage/MeleeMageAttack.cs
using UnityEngine;

public class MeleeMageController : MonoBehaviour, IEnemyAttack
{
    [Header("Configuración del Ataque")]
    [SerializeField] private float damage = 15f;
    [SerializeField] private float range = 2.5f;
    [SerializeField] private float cooldown = 2.0f;
    [SerializeField] private float windup = 0.3f;  // Tiempo antes del golpe
    [SerializeField] private float recover = 0.7f; // Tiempo después del golpe

    [Header("Referencias")]
    [SerializeField] private Animator animator; // Arrastra el Animator aquí
    [SerializeField] private HitboxDamager hitbox; // Arrastra el hitbox del arma/mano aquí

    private float _lastAttackTime;
    private EnemyCommon _enemy;

    // Propiedades para la interfaz IEnemyAttack
    public float Windup => windup;
    public float Recover => recover;
    public float Cooldown => cooldown;

    private void Awake()
    {
        _enemy = GetComponent<EnemyCommon>();
        if (animator == null) animator = GetComponentInChildren<Animator>();
    }

    public bool CanAttack()
    {
        if (Time.time < _lastAttackTime + cooldown) return false;

        var target = _enemy.Target;
        if (target == null || !target.IsValid) return false;

        float distance = Vector3.Distance(transform.position, target.AimRoot.position);
        return distance <= range;
    }

    public void DoAttack()
    {
        _lastAttackTime = Time.time;
        _enemy.LockMotionFor(windup + recover); // Bloquea el movimiento durante el ataque

        // ¡Aquí está la magia! Le decimos al Animator que active el trigger del ataque
        animator.SetTrigger("doAttack");

        // Activamos el hitbox durante la animación (usando un Invoke o Animation Events)
        // Por ejemplo, activarlo después del 'windup'
        Invoke(nameof(EnableHitbox), windup);
        Invoke(nameof(DisableHitbox), windup + 0.2f); // Desactivarlo 0.2s después
    }

    private void EnableHitbox()
    {
        if (hitbox != null)
        {
            hitbox.SetDamage(damage); // Le pasamos el daño al hitbox
            hitbox.gameObject.SetActive(true);
        }
    }

    private void DisableHitbox()
    {
        if (hitbox != null)
        {
            hitbox.gameObject.SetActive(false);
        }
    }

    public float GetAttackDuration()
    {
        throw new System.NotImplementedException();
    }
}