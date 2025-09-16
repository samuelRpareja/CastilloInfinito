using System;
using UnityEngine;

public enum MovementMode { Grounded, Flying }

[RequireComponent(typeof(CharacterController))]
public class EnemyCommon : MonoBehaviour, IDamageable, IInitializable, ITickable, IHealthReadable
{
    [Header("Stats")]
    [Min(1f)] public float maxHP = 30f;

    private float _motionUnlockTime = 0f;


    [Header("Movimiento")]
    public MovementMode movementMode = MovementMode.Grounded;
    public float moveSpeed = 3.5f;
    public float turnSpeed = 12f;
    public float gravity = -20f;
    public float stepOffset = 0.3f;

    [Header("Vuelo (solo Flying)")]
    public float verticalSpeedLimit = 5f;
    public float verticalAccel = 12f;

    [Header("Rotación")]
    public bool yawOnly = true;
    public Transform visualRoot; // opcional (normalmente no hace falta tocarlo)

    [Header("Suavizado horizontal")]
    public float accel = 18f;        // ganar velocidad
    public float decel = 22f;        // frenar
    public float stopEpsilon = 0.02f; // por debajo de esto = 0

    [Header("Refs")]
    public StateMachine fsm = new StateMachine();

    // IHealthReadable
    public float CurrentHP => _hp;
    public float MaxHP => maxHP;
    public event Action<float, float> OnHealthChanged;

    // IDamageable
    public bool IsDead => _hp <= 0f;
    public event Action OnDeath;

    // Internos
    private CharacterController _cc;
    private float _hp;
    private float _verticalVel;
    private Vector3 _hVel;   // ✅ velocidad horizontal suavizada (XZ)
    private bool _initialized;

    public ITarget Target => TargetRegistry.Instance != null ? TargetRegistry.Instance.CurrentTarget : null;

    private void Awake()
    {
        _cc = GetComponent<CharacterController>();
        if (_cc != null)
        {
            _cc.stepOffset = stepOffset;
            _cc.minMoveDistance = 0f;
            _cc.skinWidth = Mathf.Max(0.02f, _cc.skinWidth);
            _cc.slopeLimit = Mathf.Max(89f, _cc.slopeLimit);
        }
    }

    public void Initialize()
    {
        if (_cc == null) _cc = GetComponent<CharacterController>();
        _hp = Mathf.Max(1f, maxHP);
        fsm.Initialize();
        OnHealthChanged?.Invoke(_hp, maxHP);
        _initialized = true;
    }

    private void Update()
    {
        if (!_initialized) return;

        float dt = Time.deltaTime;

        fsm.Tick(dt);
        Tick(dt);

        if (movementMode == MovementMode.Grounded) ApplyGravity(dt);
        else _verticalVel = 0f; // volador: sin gravedad
    }

    public void Tick(float dt) { }

    public void TakeDamage(float amount)
    {
        if (IsDead) return;
        if (amount > 0f)
        {
            var ethereal = GetComponent<EliteEthereal>();
            if (ethereal != null && ethereal.TryNegateDamage()) return;
        }
        _hp = Mathf.Clamp(_hp - amount, 0f, maxHP);
        OnHealthChanged?.Invoke(_hp, maxHP);
        if (_hp <= 0f) OnDeath?.Invoke();
    }

    public void MultiplyMoveSpeed(float factor) { moveSpeed = Mathf.Max(0f, moveSpeed * factor); }
    public void ApplyMaxHpMultiplier(float factor)
    {
        maxHP = Mathf.Max(1f, maxHP * factor);
        if (_hp > maxHP) _hp = maxHP;
        OnHealthChanged?.Invoke(_hp, maxHP);
    }

    /// ⬇️ ⬇️  MÉTODO CLAVE (con suavizado + yaw-only)  ⬇️ ⬇️
    public void MoveTowards(Vector3 worldDir, float dt, bool faceDir = true)
    {
        if (Time.time < _motionUnlockTime)
        {
            // Solo rota (opcional), no se traslada
            if (faceDir)
            {
                Vector3 flatDir = new Vector3(worldDir.x, 0f, worldDir.z);
                if (flatDir.sqrMagnitude > 0.0001f)
                {
                    var yaw = Quaternion.LookRotation(flatDir, Vector3.up);
                    transform.rotation = Quaternion.Slerp(transform.rotation, yaw, turnSpeed * dt);
                }
            }
            _cc.Move(Vector3.up * 0f); // nada de traslación
            return;
        }


        if (_cc == null) return;
        if (dt <= 0f) dt = Time.deltaTime;

        Vector3 vel;

        if (movementMode == MovementMode.Grounded)
        {
            worldDir.y = 0f;

            // objetivo horizontal:
            Vector3 desired = worldDir.sqrMagnitude > 0.0001f ? worldDir.normalized * moveSpeed : Vector3.zero;

            // suavizado (aceleración/frenado)
            float maxDelta = ((desired.sqrMagnitude > _hVel.sqrMagnitude) ? accel : decel) * dt;
            _hVel = Vector3.MoveTowards(_hVel, desired, maxDelta);

            // umbral de parada
            if (_hVel.magnitude < stopEpsilon) _hVel = Vector3.zero;

            vel = _hVel;
            vel.y = _verticalVel;
        }
        else // Flying
        {
            // horizontal deseado
            Vector3 horizontal = worldDir; horizontal.y = 0f;
            Vector3 desired = horizontal.sqrMagnitude > 0.0001f ? horizontal.normalized * moveSpeed : Vector3.zero;

            float maxDelta = ((desired.sqrMagnitude > _hVel.sqrMagnitude) ? accel : decel) * dt;
            _hVel = Vector3.MoveTowards(_hVel, desired, maxDelta);
            if (_hVel.magnitude < stopEpsilon) _hVel = Vector3.zero;

            // vertical deseado limitado
            float desiredVy = 0f;
            if (worldDir.sqrMagnitude > 0.0001f)
                desiredVy = Mathf.Clamp(worldDir.normalized.y * verticalAccel, -verticalSpeedLimit, verticalSpeedLimit);

            vel = _hVel + Vector3.up * desiredVy;
        }

        // Rotación: SOLO yaw
        if (faceDir)
        {
            Vector3 flat = (_hVel.sqrMagnitude > 0.0001f) ? _hVel : new Vector3(worldDir.x, 0f, worldDir.z);
            if (flat.sqrMagnitude > 0.0001f)
            {
                // calcula yaw a partir del forward plano (sin euler drift)
                float yaw = Mathf.Atan2(flat.x, flat.z) * Mathf.Rad2Deg;
                Quaternion targetYaw = Quaternion.Euler(0f, yaw, 0f);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetYaw, turnSpeed * dt);
            }
        }

        _cc.Move(vel * dt);
    }

    /// Quieto de verdad
    public void Halt()
    {
        _verticalVel = 0f;
        _hVel = Vector3.zero;                 // ✅ limpia velocidad horizontal
        if (_cc != null) _cc.Move(Vector3.zero);
    }

    public void SeekTarget(ITarget target, float dt)
    {
        if (target == null || !target.IsValid) return;
        Vector3 dir = target.AimRoot.position - transform.position;
        MoveTowards(dir, dt, true);
    }

    private void ApplyGravity(float dt)
    {
        if (_cc == null) return;
        if (_cc.isGrounded && _verticalVel < 0f) _verticalVel = -2f;
        _verticalVel += gravity * dt;
        if (_verticalVel < gravity) _verticalVel = gravity;
    }

    public void LockMotionFor(float seconds)
    {
        _motionUnlockTime = Mathf.Max(_motionUnlockTime, Time.time + Mathf.Max(0f, seconds));
    }
    public float GetCurrentHP() => _hp;

    // ⬇️ Lock de yaw al final del frame (por si Animator/otros meten roll/pitch)
    private void LateUpdate()
    {
        if (!_initialized || !yawOnly) return;

        Vector3 fwd = transform.forward; fwd.y = 0f;
        if (fwd.sqrMagnitude < 0.0001f)
        {
            return; // sin forward estable, no forzamos
        }

        float yaw = Mathf.Atan2(fwd.x, fwd.z) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, yaw, 0f);

        // normalmente NO toques visualRoot: el Animator manda.
        // Si aun así tu FBX mete roll, podrías “alinearlo”:
        // if (visualRoot) visualRoot.localRotation = Quaternion.identity;
    }
}
