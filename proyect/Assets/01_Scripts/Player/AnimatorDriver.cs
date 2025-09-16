using UnityEngine;

[RequireComponent(typeof(Animator))]
public class AnimatorDriver : MonoBehaviour
{
    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void UpdateMovementParams(float horizontal, float vertical)
    {
        if (animator == null)
        {
            return;
        }

        animator.SetFloat("VelX", horizontal);
        animator.SetFloat("VelY", vertical);
    }

    public void TriggerAttack()
    {
        if (animator == null)
        {
            return;
        }

        animator.SetTrigger("golpeo");
    }
}


