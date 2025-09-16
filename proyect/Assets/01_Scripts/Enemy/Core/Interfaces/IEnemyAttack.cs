using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IEnemyAttack
{
    bool CanAttack();
    void DoAttack();
    float GetAttackDuration();

    float Cooldown { get; }

}
