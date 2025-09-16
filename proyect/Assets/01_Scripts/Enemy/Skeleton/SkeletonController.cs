using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkeletonController : MonoBehaviour, IInitializable
{
    [SerializeField] private float aggroRange = 8f;

    private EnemyCommon enemy;
    private IEnemyAttack attack;

    public void Initialize()
    {
        enemy = GetComponent<EnemyCommon>();
        attack = GetComponent<IEnemyAttack>();

        enemy.Initialize();
        enemy.fsm.Set(new IdleState(enemy, aggroRange));

        // En muerte → DeadState
        enemy.OnDeath += () => enemy.fsm.Set(new DeadState(enemy));
    }

    private void Update()
    {
        if (enemy.IsDead) return;

        // Si tiene target y puede atacar
        if (attack != null && attack.CanAttack())
            enemy.fsm.Set(new AttackState(enemy, aggroRange, attack));
    }
}
