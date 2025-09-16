using UnityEngine;

/// <summary>
/// Interface for objects that can be moved
/// Single Responsibility Principle (SRP)
/// </summary>
public interface IMovable
{
    void Move(Vector3 direction, float speed);
    void MoveTo(Vector3 position);
    void Stop();
    bool IsMoving { get; }
}
