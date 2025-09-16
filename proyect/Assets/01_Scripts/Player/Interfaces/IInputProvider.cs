using UnityEngine;

/// <summary>
/// Interface for input providers
/// Dependency Inversion Principle (DIP)
/// </summary>
public interface IInputProvider
{
    Vector2 GetMovementInput();
    Vector2 GetMouseInput();
    bool GetActionInput();
}
