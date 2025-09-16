using UnityEngine;

public interface IPlayerInput
{
    float Horizontal { get; }
    float Vertical { get; }
    bool AttackPressedThisFrame { get; }

    void Refresh();
}


