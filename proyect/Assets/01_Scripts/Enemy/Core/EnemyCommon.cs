using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Núcleo común de enemigo: HP/daño, movimiento (CharacterController),
/// rotación hacia la dirección de movimiento y orquestación de FSM.
/// Implementa IDamageable, IInitializable, ITickable e IHealthReadable.
/// </summary>
[RequireComponent(typeof(CharacterController))]
public class EnemyCommon : MonoBehaviour, IDamageable, IInitializable, ITickable, IHealthReadable
{
    [Header("Stats (puedes sobreescribir vía SO)")]
    [Min(1f)] public float maxHP = 30f;
    [Min(0f)] public float moveSpeed = 3f;
    public float gravity = -20f;

    [Header("Refs")]
    public StateMachine fsm = new StateMachine();

    // IHealthReadable
    public float CurrentHP => _hp;
    public float MaxHP => maxHP;
    public event Action<float, float> OnHealthChanged;

    // IDamageable
    public bool IsDead => _hp <= 0f;
    public event Action OnDeath;

    // Utilidades internas
    private CharacterController _cc;
    private float _hp;
    private float _verticalVel;

    // Acceso al target actual (player real o PlayerProxy)
    public ITarget Target => TargetRegistry.Instance?.CurrentTarget;

    public void Initialize()
    {
        if (_cc == null) _cc = GetComponent<CharacterController>();
        _hp = Mathf.Max(1f, maxHP);
        fsm.Initialize();
        OnHealthChanged?.Invoke(_hp, maxHP);
    }

    public void Tick(float dt)
    {
        ApplyGravity(dt); // la lógica de IA vive en los Estados
    }

    private void Update()
    {
        float dt = Time.deltaTime;
        fsm.Tick(dt);
        Tick(dt);
    }

    /// <summary>
    /// Aplica daño positivo o curación (daño negativo). Gestiona Ethereal si existe.
    /// </summary>
    public void TakeDamage(float amount)
    {
        if (IsDead) return;

        // Hook opcional: EliteEthereal puede negar daño
        if (amount > 0f)
        {
            var ethereal = GetComponent<EliteEthereal>();
            if (ethereal != null && ethereal.TryNegateDamage())
                return;
        }

        _hp -= amount; // amount negativo cura
        _hp = Mathf.Clamp(_hp, 0f, maxHP);
        OnHealthChanged?.Invoke(_hp, maxHP);

        if (_hp <= 0f)
        {
            OnDeath?.Invoke();
            // Si usas DeadState, cámbialo desde el controlador del enemigo.
            // (Destruye/Desactiva el GO desde ese estado para mantener orden)
        }
    }


    public void MultiplyMoveSpeed(float factor)
    {
        moveSpeed = Mathf.Max(0f, moveSpeed * factor);
    }

    public void ApplyMaxHpMultiplier(float factor)
    {
        // factor < 1 reduce el HP máximo (p. ej. 0.9f = -10%)
        float oldMax = maxHP;
        maxHP = Mathf.Max(1f, maxHP * factor);

        // Si el HP actual excede el nuevo máximo, clámpalo
        if (_hp > maxHP) _hp = maxHP;

        // Notificar a la UI
        OnHealthChanged?.Invoke(_hp, maxHP);
    }

    /// <summary>
    /// Movimiento plano + orientación suave hacia worldDir (Y la maneja la gravedad).
    /// Llama desde tus estados (Chase/Attack según convenga).
    /// </summary>
    public void MoveTowards(Vector3 worldDir, float dt)
    {
        worldDir.y = 0f;
        Vector3 vel = worldDir.sqrMagnitude > 0.0001f ? worldDir.normalized * moveSpeed : Vector3.zero;
        vel.y = _verticalVel;

        _cc.Move(vel * dt);

        if (worldDir.sqrMagnitude > 0.0001f)
        {
            var targetRot = Quaternion.LookRotation(worldDir);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, dt * 12f);
        }
    }



    private void ApplyGravity(float dt)
    {
        if (_cc.isGrounded && _verticalVel < 0f) _verticalVel = -2f;
        _verticalVel += gravity * dt;
    }

    /// <summary>
    /// Getter explícito por si un UI externo prefiere consulta directa.
    /// </summary>
    public float GetCurrentHP() => _hp;
}