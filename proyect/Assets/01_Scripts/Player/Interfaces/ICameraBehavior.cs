using UnityEngine;

/// <summary>
/// Interface for camera behaviors
/// Interface Segregation Principle (ISP)
/// </summary>
public interface ICameraBehavior
{
    void UpdateCamera(Transform camera, ICameraTarget target, Vector3 offset, float mouseX, float mouseY, float followSpeed);
}
