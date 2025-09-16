using UnityEngine;

/// <summary>
/// Unity Animator implementation
/// Single Responsibility Principle (SRP)
/// </summary>
public class AnimatorController : MonoBehaviour, IAnimatorController
{
    private Animator animator;
    
    private void Awake()
    {
        animator = GetComponent<Animator>();
    }
    
    public void UpdateMovement(float velX, float velY, bool isMoving)
    {
        if (animator == null) return;
        
        // Only set parameters that exist
        if (HasParameter("VelX"))
            animator.SetFloat("VelX", velX);
        if (HasParameter("VelY"))
            animator.SetFloat("VelY", velY);
        if (HasParameter("IsMoving"))
            animator.SetBool("IsMoving", isMoving);
        if (HasParameter("Speed"))
            animator.SetFloat("Speed", new Vector2(velX, velY).magnitude);
    }
    
    public void SetTrigger(string triggerName)
    {
        if (animator != null && HasParameter(triggerName))
            animator.SetTrigger(triggerName);
    }
    
    public void SetBool(string parameterName, bool value)
    {
        if (animator != null && HasParameter(parameterName))
            animator.SetBool(parameterName, value);
    }
    
    public void SetFloat(string parameterName, float value)
    {
        if (animator != null && HasParameter(parameterName))
            animator.SetFloat(parameterName, value);
    }
    
    public void SetInt(string parameterName, int value)
    {
        if (animator != null && HasParameter(parameterName))
            animator.SetInteger(parameterName, value);
    }
    
    private bool HasParameter(string paramName)
    {
        if (animator == null) return false;
        
        foreach (AnimatorControllerParameter param in animator.parameters)
        {
            if (param.name == paramName)
                return true;
        }
        return false;
    }
}
