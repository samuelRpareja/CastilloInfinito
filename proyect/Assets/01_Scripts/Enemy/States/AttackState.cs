using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackState : IState
{
    private readonly EnemyCommon enemy;
    private readonly float aggroRange;
    private readonly IEnemyAttack attack;

    public AttackState(EnemyCommon enemy, float aggroRange, IEnemyAttack attack)
    {
        this.enemy = enemy;
        this.aggroRange = aggroRange;
        this.attack = attack;
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

        // Si está demasiado lejos → volver a Chase
        if (dist > aggroRange)
        {
            enemy.fsm.Set(new ChaseState(enemy, aggroRange));
            return;
        }

        // Atacar si puede
        if (attack.CanAttack())
        {
            attack.DoAttack();
        }
    }

    public void Exit() { }
}
