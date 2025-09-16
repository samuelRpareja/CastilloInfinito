using UnityEngine;

/// <summary>
/// Third Person Camera Behavior implementation
/// Single Responsibility Principle (SRP)
/// </summary>
public class ThirdPersonCameraBehavior : MonoBehaviour, ICameraBehavior
{
    public void UpdateCamera(Transform camera, ICameraTarget target, Vector3 offset, float mouseX, float mouseY, float followSpeed)
    {
        if (target == null) return;
        
        // Calculate rotation
        Quaternion rotation = Quaternion.Euler(mouseY, mouseX, 0);
        
        // Calculate target position
        Vector3 targetPosition = target.Position + rotation * offset;
        
        // Apply smooth following
        camera.position = Vector3.Lerp(camera.position, targetPosition, followSpeed * Time.deltaTime);
        
        // Look at target
        camera.LookAt(target.Position);
    }
}
