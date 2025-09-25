using UnityEngine;
using Unity.Netcode;

[RequireComponent(typeof(NetworkObject))]
public class NetworkPlayer : NetworkBehaviour
{
    [Header("Componentes del Jugador")]
    [SerializeField] private PlayerController playerController;
    [SerializeField] private Health playerHealth;
    [SerializeField] private Animator playerAnimator;
    
    [Header("Configuración de Red")]
    public NetworkVariable<string> playerName = new NetworkVariable<string>("Jugador");
    public NetworkVariable<Color> playerColor = new NetworkVariable<Color>(Color.white);
    
    // Variables sincronizadas para movimiento
    public NetworkVariable<Vector3> networkPosition = new NetworkVariable<Vector3>();
    public NetworkVariable<Quaternion> networkRotation = new NetworkVariable<Quaternion>();
    public NetworkVariable<bool> isMoving = new NetworkVariable<bool>();
    public NetworkVariable<bool> isAttacking = new NetworkVariable<bool>();
    
    // Referencias locales
    private Vector3 lastPosition;
    private Quaternion lastRotation;
    private bool lastMoving;
    private bool lastAttacking;
    
    private void Awake()
    {
        // Obtener componentes si no están asignados
        if (playerController == null)
            playerController = GetComponent<PlayerController>();
        if (playerHealth == null)
            playerHealth = GetComponent<Health>();
        if (playerAnimator == null)
            playerAnimator = GetComponent<Animator>();
    }
    
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        
        // Solo el cliente local puede controlar este jugador
        if (IsOwner)
        {
            // Configurar cámara para seguir al jugador local
            SetupLocalPlayer();
        }
        else
        {
            // Deshabilitar input para jugadores remotos
            DisableRemotePlayerInput();
        }
        
        // Suscribirse a eventos de salud
        if (playerHealth != null)
        {
            playerHealth.OnDeath += OnPlayerDeath;
        }
    }
    
    private void Update()
    {
        if (IsOwner)
        {
            // Solo el jugador local actualiza las variables de red
            UpdateNetworkVariables();
        }
        else
        {
            // Los jugadores remotos interpolan hacia las posiciones de red
            InterpolateRemotePlayer();
        }
    }
    
    private void UpdateNetworkVariables()
    {
        // Actualizar posición y rotación
        Vector3 currentPos = transform.position;
        Quaternion currentRot = transform.rotation;
        
        // Detectar si el jugador se está moviendo
        bool moving = Vector3.Distance(currentPos, lastPosition) > 0.01f;
        
        // Solo sincronizar si hay cambios significativos
        if (Vector3.Distance(currentPos, networkPosition.Value) > 0.1f)
        {
            UpdatePositionServerRpc(currentPos, currentRot, moving);
        }
        
        lastPosition = currentPos;
        lastRotation = currentRot;
        lastMoving = moving;
    }
    
    [ServerRpc]
    private void UpdatePositionServerRpc(Vector3 position, Quaternion rotation, bool moving)
    {
        networkPosition.Value = position;
        networkRotation.Value = rotation;
        isMoving.Value = moving;
    }
    
    private void InterpolateRemotePlayer()
    {
        // Interpolar suavemente hacia la posición de red
        if (Vector3.Distance(transform.position, networkPosition.Value) > 0.1f)
        {
            transform.position = Vector3.Lerp(transform.position, networkPosition.Value, Time.deltaTime * 10f);
            transform.rotation = Quaternion.Lerp(transform.rotation, networkRotation.Value, Time.deltaTime * 10f);
        }
        
        // Actualizar animaciones
        if (playerAnimator != null)
        {
            playerAnimator.SetBool("IsMoving", isMoving.Value);
            playerAnimator.SetBool("IsAttacking", isAttacking.Value);
        }
    }
    
    private void SetupLocalPlayer()
    {
        Debug.Log($"🎮 Configurando jugador local: {playerName.Value}");
        
        // Configurar cámara para seguir al jugador
        Camera mainCamera = Camera.main;
        if (mainCamera != null)
        {
            ThirdPersonCameraBehavior cameraBehavior = mainCamera.GetComponent<ThirdPersonCameraBehavior>();
            if (cameraBehavior != null)
            {
                // Comentar temporalmente hasta que se implemente SetTarget
                // cameraBehavior.SetTarget(transform);
                Debug.Log("Cámara configurada para seguir al jugador");
            }
        }
        
        // Configurar UI para el jugador local
        SetupLocalPlayerUI();
    }
    
    private void DisableRemotePlayerInput()
    {
        // El PlayerInputController se encarga de deshabilitar el input
        Debug.Log($"👥 Jugador remoto configurado: {playerName.Value}");
    }
    
    private void SetupLocalPlayerUI()
    {
        // Aquí puedes configurar UI específica para el jugador local
        // Por ejemplo, mostrar la barra de vida, controles, etc.
    }
    
    private void OnPlayerDeath()
    {
        if (IsOwner)
        {
            Debug.Log("💀 Tu jugador ha muerto!");
            // El GameManager local manejará el game over
        }
        else
        {
            Debug.Log($"💀 {playerName.Value} ha muerto!");
        }
    }
    
    // Método para sincronizar ataques
    [ServerRpc]
    public void TriggerAttackServerRpc()
    {
        TriggerAttackClientRpc();
    }

    [ClientRpc]
    private void TriggerAttackClientRpc()
    {
        if (playerAnimator != null)
        {
            playerAnimator.SetTrigger("Attack");
        }
        isAttacking.Value = true;
        
        // Resetear el estado de ataque después de un tiempo
        Invoke(nameof(ResetAttackState), 0.5f);
    }
    
    private void ResetAttackState()
    {
        isAttacking.Value = false;
    }
    
    // Método para sincronizar daño
    [ServerRpc]
    public void TakeDamageServerRpc(float damage)
    {
        if (playerHealth != null)
        {
            playerHealth.TakeDamage(damage);
        }
    }
    
    public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn();
        Debug.Log($"👋 Jugador desconectado: {playerName.Value}");
    }
    
    private void OnDestroy()
    {
        if (playerHealth != null)
        {
            playerHealth.OnDeath -= OnPlayerDeath;
        }
    }
}
