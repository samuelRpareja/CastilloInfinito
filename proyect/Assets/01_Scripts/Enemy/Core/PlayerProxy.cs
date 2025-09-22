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
    
    // Propiedades para acceso a HP
    public float CurrentHP => _hp;
    public float MaxHP => maxHP;

    public Transform AimRoot => aimRoot != null ? aimRoot : transform;
    public bool IsValid => !IsDead && isActiveAndEnabled;

    private void OnEnable()
    {
        _hp = maxHP;
        Debug.Log($" PlayerProxy: OnEnable - {name} (AimRoot: {(aimRoot != null ? aimRoot.name : "NULL")})");
        TargetRegistry.Instance?.Register(this);
    }

    private void OnDisable()
    {
        TargetRegistry.Instance?.Unregister(this);
    }

    public void TakeDamage(float amount)
    {
        Debug.Log($"PlayerProxy: Recibiendo {amount} de daño. HP: {_hp} -> {_hp - amount}");
        
        if (IsDead) return;
        _hp -= amount;
        _hp = Mathf.Clamp(_hp, 0f, maxHP);
        
        Debug.Log($"PlayerProxy: HP actual: {_hp}/{maxHP}");
        
        if (_hp <= 0f) 
        {
            Debug.Log("PlayerProxy: ¡JUGADOR MUERTO!");
            OnDeath?.Invoke();
        }
    }
}