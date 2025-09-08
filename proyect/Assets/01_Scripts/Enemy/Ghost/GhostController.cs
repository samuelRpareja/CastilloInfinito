using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostController : MonoBehaviour, IInitializable
{
    [SerializeField] private float aggroRange = 10f;
    private EnemyCommon enemy;
    private IEnemyAttack attack;
    private GhostPhase phase;

    public void Initialize()
    {
        enemy = GetComponent<EnemyCommon>();
        attack = GetComponent<IEnemyAttack>();
        phase = GetComponent<GhostPhase>();

        enemy.Initialize();
        enemy.fsm.Set(new IdleState(enemy, aggroRange));

        enemy.OnDeath += () => enemy.fsm.Set(new DeadState(enemy));
    }

    private void Update()
    {
        if (enemy.IsDead) return;

        if (attack != null && attack.CanAttack())
            enemy.fsm.Set(new AttackState(enemy, aggroRange, attack));

        // Ejemplo: fantasma “phasea” si hay obstáculos
        if (phase != null && phase.CanPhase && Random.value < 0.01f)
        {
            phase.DoPhase();
        }
    }
}
