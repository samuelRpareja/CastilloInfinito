using UnityEngine;

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
    
    // Input directo para fallback
    private float horizontal;
    private float vertical;
    private bool attackPressed;

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

        // Obtener input
        if (playerInput != null)
        {
            playerInput.Refresh();
            horizontal = playerInput.Horizontal;
            vertical = playerInput.Vertical;
            attackPressed = playerInput.AttackPressedThisFrame;
        }
        else
        {
            // Fallback directo a teclado
            horizontal = Input.GetAxis("Horizontal");
            vertical = Input.GetAxis("Vertical");
            attackPressed = Input.GetKeyDown(KeyCode.Space);
        }

        // Debug del input
        if (Mathf.Abs(horizontal) > 0.01f || Mathf.Abs(vertical) > 0.01f)
        {
            Debug.Log($"PLAYER CONTROLLER: H={horizontal:F2}, V={vertical:F2}, playerInput={playerInput != null}");
        }

        animatorDriver?.UpdateMovementParams(horizontal, vertical);

        if (attackPressed)
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

        // Simplificar - siempre permitir movimiento
        movementController?.Move(horizontal, vertical, true);
    }
}


