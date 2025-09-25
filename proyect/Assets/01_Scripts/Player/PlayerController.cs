using UnityEngine;
using Unity.Netcode;

[RequireComponent(typeof(SimpleMovementController))]
[RequireComponent(typeof(AnimatorDriver))]
[RequireComponent(typeof(SimpleAttacker))]
[RequireComponent(typeof(Health))]
public class PlayerController : NetworkBehaviour
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
        // Solo el jugador local puede procesar input
        if (!IsOwner)
        {
            return;
        }

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
                // Sincronizar ataque en red
                TriggerAttackServerRpc();
            }
        }
    }

    private void FixedUpdate()
    {
        // Solo el jugador local puede moverse
        if (!IsOwner)
        {
            return;
        }

        // Si el jugador está muerto, no moverse
        if (health != null && !health.IsAlive())
        {
            return;
        }

        // Simplificar - siempre permitir movimiento
        movementController?.Move(horizontal, vertical, true);
    }

    // Método para sincronizar ataques en red
    [ServerRpc]
    private void TriggerAttackServerRpc()
    {
        TriggerAttackClientRpc();
    }

    [ClientRpc]
    private void TriggerAttackClientRpc()
    {
        if (animatorDriver != null)
        {
            animatorDriver.TriggerAttack();
        }
    }

    // Método para sincronizar daño en red
    [ServerRpc]
    public void TakeDamageServerRpc(float damage)
    {
        if (health != null)
        {
            health.TakeDamage(damage);
        }
    }
}


