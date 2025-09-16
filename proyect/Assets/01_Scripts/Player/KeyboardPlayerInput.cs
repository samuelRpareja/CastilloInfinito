using UnityEngine;

public class KeyboardPlayerInput : MonoBehaviour, IPlayerInput
{
    public float Horizontal { get; private set; }
    public float Vertical { get; private set; }
    public bool AttackPressedThisFrame { get; private set; }

    public void Refresh()
    {
        Horizontal = Input.GetAxis("Horizontal");
        Vertical = Input.GetAxis("Vertical");
        AttackPressedThisFrame = Input.GetKeyDown(KeyCode.Return);
    }
}


