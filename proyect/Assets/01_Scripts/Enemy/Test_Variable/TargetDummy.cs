using UnityEngine;

public class TargetDummy : MonoBehaviour, ITarget, IDamageable
{
    public Transform aimRoot;
    public float maxHP = 50f;
    public bool logDamage = true;

    private float _hp;
    public bool IsValid => true;
    public Transform AimRoot => aimRoot != null ? aimRoot : transform;
    public bool IsDead => _hp <= 0f;
    public event System.Action OnDeath;

    void Awake()
    {
        if (aimRoot == null) aimRoot = transform;
        _hp = maxHP;
    }

    public void TakeDamage(float amount)
    {
        if (IsDead) return;
        _hp = Mathf.Clamp(_hp - amount, 0f, maxHP);
        if (logDamage) Debug.LogWarning($"[Dummy] Daño {amount} → HP: {_hp}/{maxHP}");
        if (_hp <= 0f) OnDeath?.Invoke();
    }
}
