using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChaseState : IState
{
    private readonly EnemyCommon enemy;
    private readonly float aggroRange;

    public ChaseState(EnemyCommon enemy, float aggroRange)
    {
        this.enemy = enemy;
        this.aggroRange = aggroRange;
    }

    public void Enter() { }

    public void Tick(float dt)
    {
        var tgt = enemy.Target;
        if (tgt == null || !tgt.IsValid)
        {
            enemy.fsm.Set(new IdleState(enemy, aggroRange));
            return;
        }

        Vector3 dir = (tgt.AimRoot.position - enemy.transform.position);
        float dist = dir.magnitude;

        // Perseguir
        enemy.MoveTowards(dir, dt);

        // Si está muerto o demasiado lejos → volver a Idle
        if (dist > aggroRange * 1.5f)
        {
            enemy.fsm.Set(new IdleState(enemy, aggroRange));
            return;
        }

        // Si puede atacar, cambiar a AttackState
        var attack = enemy.GetComponent<IEnemyAttack>();
        if (attack != null && attack.CanAttack())
        {
            enemy.fsm.Set(new AttackState(enemy, aggroRange, attack));
        }
    }

    public void Exit() { }
}
