using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleState : IState
{
    private readonly EnemyCommon enemy;
    private readonly float aggroRange;

    public IdleState(EnemyCommon enemy, float aggroRange)
    {
        this.enemy = enemy;
        this.aggroRange = aggroRange;
    }

    public void Enter() { }

    public void Tick(float dt)
    {
        var tgt = enemy.Target;
        if (tgt != null && tgt.IsValid)
        {
            float dist = Vector3.Distance(enemy.transform.position, tgt.AimRoot.position);
            if (dist <= aggroRange)
            {
                // Cambiar a persecución
                enemy.fsm.Set(new ChaseState(enemy, aggroRange));
            }
        }
    }

    public void Exit() { }
}
