using UnityEngine;

/// <summary>
/// Interface for camera targets
/// Interface Segregation Principle (ISP)
/// </summary>
public interface ICameraTarget
{
    Transform Transform { get; }
    Vector3 Position { get; }
    Vector3 Forward { get; }
    bool IsActive { get; }
}
