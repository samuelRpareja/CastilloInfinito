using UnityEngine;

/// <summary>
/// Estado de ataque que maneja la lógica de combate y transiciones correctas
/// </summary>
public class AttackState : IState
{
    private readonly EnemyCommon _enemy;
    private readonly IEnemyAttack _attack;
    private readonly float _aggroRange;
    private float _attackEndTime;
    private bool _attackExecuted;

    public AttackState(EnemyCommon enemy, float aggroRange, IEnemyAttack attack)
    {
        _enemy = enemy;
        _attack = attack;
        _aggroRange = aggroRange;
    }

    public void Enter()
    {
        Debug.Log($"<color=magenta>[AttackState] Enter() - {_enemy.name}</color>");
        Debug.Log($"   🎯 Iniciando ataque a las {Time.time:F2}s");

        if (_attack != null)
        {
            Debug.Log($"   ✅ Ejecutando ataque: {_attack.GetType().Name}");
            Debug.Log($"   ⏰ Cooldown: {_attack.Cooldown:F2}s");
            Debug.Log($"   ⏱️ Duración de ataque: {_attack.GetAttackDuration():F2}s");
            
            _attack.DoAttack();
            
            // Configurar timer para terminar el ataque
            _attackEndTime = Time.time + _attack.GetAttackDuration();
            _attackExecuted = true;
            
            Debug.Log($"   ⏱️ Ataque terminará a las {_attackEndTime:F2}s");
        }
        else
        {
            Debug.LogError("   ❌ ¡ERROR! La referencia 'attack' ES NULA.");
            _attackEndTime = Time.time + 0.5f; // Fallback
            _attackExecuted = false;
        }
    }

    public void Tick(float dt)
    {
        // Verificar si el ataque ha terminado
        if (Time.time >= _attackEndTime)
        {
            Debug.Log($"<color=cyan>[AttackState] Ataque completado - {_enemy.name}</color>");
            
            // Verificar si el target sigue siendo válido
            var target = _enemy.Target;
            if (target != null && target.IsValid)
            {
                float dist = Vector3.Distance(_enemy.transform.position, target.AimRoot.position);
                Debug.Log($"   📏 Distancia al target: {dist:F2}");
                
                // Si el target está cerca, volver a perseguir
                if (dist <= _aggroRange)
                {
                    Debug.Log($"   ✅ Target cerca - Volviendo a persecución");
                    _enemy.fsm.Set(new GhostChaseState(_enemy, _aggroRange));
                }
                else
                {
                    Debug.Log($"   ❌ Target lejos - Volviendo a Idle");
                    _enemy.fsm.Set(new IdleStateDebug(_enemy, _aggroRange));
                }
            }
            else
            {
                Debug.Log($"   ❌ Target inválido - Volviendo a Idle");
                _enemy.fsm.Set(new IdleStateDebug(_enemy, _aggroRange));
            }
        }
    }

    public void Exit()
    {
        Debug.Log($"<color=magenta>[AttackState] Exit() - {_enemy.name}</color>");
        _attackExecuted = false;
    }
}