using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MageController : MonoBehaviour, IInitializable
{
    [SerializeField] private float minRange = 6f;
    [SerializeField] private float maxRange = 12f;

    private EnemyCommon enemy;
    private IEnemyAttack attack;

    public void Initialize()
    {
        enemy = GetComponent<EnemyCommon>();
        attack = GetComponent<IEnemyAttack>();

        enemy.Initialize();

        if (attack != null)
            enemy.fsm.Set(new RangedKiteState(enemy, attack, minRange, maxRange));
        else
            enemy.fsm.Set(new IdleState(enemy, maxRange));

        enemy.OnDeath += () => enemy.fsm.Set(new DeadState(enemy));
    }
}
