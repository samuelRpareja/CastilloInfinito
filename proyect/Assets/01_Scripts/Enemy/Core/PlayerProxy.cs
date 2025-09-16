using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerProxy : MonoBehaviour, IDamageable, ITarget
{
    [SerializeField] private Transform aimRoot;
    [SerializeField] private float maxHP = 100f;

    private float _hp;
    public bool IsDead => _hp <= 0f;
    public event Action OnDeath;

    public Transform AimRoot => aimRoot != null ? aimRoot : transform;
    public bool IsValid => !IsDead && isActiveAndEnabled;

    private void OnEnable()
    {
        _hp = maxHP;
        TargetRegistry.Instance?.Register(this);
    }

    private void OnDisable()
    {
        TargetRegistry.Instance?.Unregister(this);
    }

    public void TakeDamage(float amount)
    {
        if (IsDead) return;
        _hp -= amount;
        _hp = Mathf.Clamp(_hp, 0f, maxHP);
        if (_hp <= 0f) OnDeath?.Invoke();
    }
}