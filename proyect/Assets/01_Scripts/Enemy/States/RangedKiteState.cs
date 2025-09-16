using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedKiteState : IState
{
    private readonly EnemyCommon enemy;
    private readonly IEnemyAttack attack;
    private readonly float minRange;
    private readonly float maxRange;

    public RangedKiteState(EnemyCommon enemy, IEnemyAttack attack, float minRange = 6f, float maxRange = 12f)
    {
        this.enemy = enemy;
        this.attack = attack;
        this.minRange = minRange;
        this.maxRange = maxRange;
    }

    public void Enter() { }

    public void Tick(float dt)
    {
        var tgt = enemy.Target;
        if (tgt == null || !tgt.IsValid)
        {
            enemy.fsm.Set(new IdleState(enemy, maxRange));
            return;
        }

        Vector3 dir = (tgt.AimRoot.position - enemy.transform.position);
        float dist = dir.magnitude;

        if (dist < minRange)
        {
            // Alejarse
            enemy.MoveTowards(-dir, dt);
        }
        else if (dist > maxRange)
        {
            // Acercarse
            enemy.MoveTowards(dir, dt);
        }

        if (attack.CanAttack())
        {
            attack.DoAttack();
        }
    }

    public void Exit() { }
}
