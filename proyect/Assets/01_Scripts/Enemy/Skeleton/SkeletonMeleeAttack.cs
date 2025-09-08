using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkeletonMeleeAttack : MeleeAttack
{
    [Header("Animaciones / VFX")]
    [SerializeField] private Animator animator;   // opcional
    [SerializeField] private string attackTrigger = "Attack";
    [SerializeField] private ParticleSystem attackVfx; // opcional

    protected override void OnAttack()
    {
        base.OnAttack();

        // Animación de ataque
        if (animator != null && !string.IsNullOrEmpty(attackTrigger))
            animator.SetTrigger(attackTrigger);

        // VFX de ataque
        if (attackVfx != null)
            attackVfx.Play();
    }
}
