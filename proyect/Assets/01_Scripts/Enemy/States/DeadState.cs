using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeadState : IState
{
    private readonly EnemyCommon enemy;

    public DeadState(EnemyCommon enemy)
    {
        this.enemy = enemy;
    }

    public void Enter()
    {
        // Aquí podrías desactivar colisiones, reproducir animaciones, etc.
        var cc = enemy.GetComponent<CharacterController>();
        if (cc) cc.enabled = false;

        var col = enemy.GetComponent<Collider>();
        if (col) col.enabled = false;

        // Destruir después de unos segundos
        GameObject.Destroy(enemy.gameObject, 3f);
    }

    public void Tick(float dt) { }

    public void Exit() { }
}
