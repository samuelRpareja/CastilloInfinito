using UnityEngine;
using Unity.Netcode;

public class PlayerInputController : NetworkBehaviour
{
    [Header("Componentes de Input")]
    [SerializeField] private PlayerController playerController;
    
    private void Awake()
    {
        // Obtener componentes si no están asignados
        if (playerController == null)
            playerController = GetComponent<PlayerController>();
    }
    
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        
        // Solo el dueño puede controlar este jugador
        if (!IsOwner)
        {
            DisableInput();
        }
        else
        {
            EnableInput();
        }
    }
    
    private void EnableInput()
    {
        Debug.Log($"🎮 Habilitando input para jugador local: {gameObject.name}");
        
        if (playerController != null)
        {
            playerController.enabled = true;
        }
        
        // Habilitar todos los componentes de input
        var inputComponents = GetComponents<MonoBehaviour>();
        foreach (var component in inputComponents)
        {
            if (component is IPlayerInput && component != this)
            {
                component.enabled = true;
            }
        }
    }
    
    private void DisableInput()
    {
        Debug.Log($"🔒 Deshabilitando input para jugador remoto: {gameObject.name}");
        
        if (playerController != null)
        {
            playerController.enabled = false;
        }
        
        // Deshabilitar todos los componentes de input
        var inputComponents = GetComponents<MonoBehaviour>();
        foreach (var component in inputComponents)
        {
            if (component is IPlayerInput && component != this)
            {
                component.enabled = false;
            }
        }
    }
    
    private void Update()
    {
        if (!IsOwner)
        {
            return;
        }
        
    }
}