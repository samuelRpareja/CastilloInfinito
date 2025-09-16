using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class StateMachine : IInitializable, ITickable
{
    private IState current;

    public void Set(IState next)
    {
        if (current == next) return;
        current?.Exit();
        current = next;
        current?.Enter();
    }

    public void Initialize() => current?.Enter();

    public void Tick(float dt) => current?.Tick(dt);
}
