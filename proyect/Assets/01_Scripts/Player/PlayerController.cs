using UnityEngine;

[RequireComponent(typeof(KeyboardPlayerInput))]
[RequireComponent(typeof(SimpleMovementController))]
[RequireComponent(typeof(AnimatorDriver))]
[RequireComponent(typeof(SimpleAttacker))]
[RequireComponent(typeof(Health))]
public class PlayerController : MonoBehaviour
{
    private IPlayerInput playerInput;
    private IMovementController movementController;
    private AnimatorDriver animatorDriver;
    private IAttacker attacker;
    private Health health;

    private void Awake()
    {
        playerInput = GetComponent<IPlayerInput>();
        movementController = GetComponent<IMovementController>();
        animatorDriver = GetComponent<AnimatorDriver>();
        attacker = GetComponent<IAttacker>();
        health = GetComponent<Health>();
    }

    private void Update()
    {
        // Si el jugador está muerto, no hacer nada
        if (health != null && !health.IsAlive())
        {
            return;
        }

        playerInput?.Refresh();

        animatorDriver?.UpdateMovementParams(playerInput?.Horizontal ?? 0f, playerInput?.Vertical ?? 0f);

        if (playerInput != null && playerInput.AttackPressedThisFrame)
        {
            attacker?.TryAttack();
            if (attacker != null && attacker.IsAttacking)
            {
                animatorDriver?.TriggerAttack();
            }
        }
    }

    private void FixedUpdate()
    {
        // Si el jugador está muerto, no moverse
        if (health != null && !health.IsAlive())
        {
            return;
        }

        bool canMove = attacker == null || !attacker.IsAttacking;
        movementController?.Move(playerInput?.Horizontal ?? 0f, playerInput?.Vertical ?? 0f, canMove);
    }
}


