using UnityEngine;

/// <summary>
/// Script que reemplaza automáticamente KeyboardPlayerInput con SimpleJoystickInput
/// Se agrega a cualquier GameObject en la escena
/// </summary>
public class InputReplacer : MonoBehaviour
{
    [Header("Configuración")]
    [SerializeField] private bool reemplazarAutomaticamente = true;
    
    private void Start()
    {
        if (reemplazarAutomaticamente)
        {
            ReemplazarInput();
        }
    }
    
    /// <summary>
    /// Reemplaza KeyboardPlayerInput con SimpleJoystickInput
    /// </summary>
    [ContextMenu("Reemplazar Input")]
    public void ReemplazarInput()
    {
        Debug.Log("InputReplacer: Iniciando reemplazo de input...");
        
        // Buscar PlayerController
        PlayerController playerController = FindObjectOfType<PlayerController>();
        if (playerController == null)
        {
            Debug.LogError("InputReplacer: No se encontró PlayerController");
            return;
        }
        
        Debug.Log($"InputReplacer: PlayerController encontrado - {playerController.name}");
        
        // Verificar si ya tiene SimpleJoystickInput
        SimpleJoystickInput existingInput = playerController.GetComponent<SimpleJoystickInput>();
        if (existingInput != null)
        {
            Debug.Log("InputReplacer: SimpleJoystickInput ya existe");
            return;
        }
        
        // Remover KeyboardPlayerInput si existe
        KeyboardPlayerInput oldInput = playerController.GetComponent<KeyboardPlayerInput>();
        if (oldInput != null)
        {
            DestroyImmediate(oldInput);
            Debug.Log("InputReplacer: KeyboardPlayerInput removido");
        }
        
        // Agregar SimpleJoystickInput
        SimpleJoystickInput newInput = playerController.gameObject.AddComponent<SimpleJoystickInput>();
        
        // Buscar y configurar el joystick
        VirtualJoystick joystick = FindObjectOfType<VirtualJoystick>();
        if (joystick != null)
        {
            newInput.SetJoystick(joystick);
            Debug.Log("InputReplacer: Joystick configurado");
        }
        else
        {
            Debug.LogWarning("InputReplacer: No se encontró VirtualJoystick");
        }
        
        Debug.Log("InputReplacer: Reemplazo completado exitosamente");
    }
    
    /// <summary>
    /// Verifica el estado actual
    /// </summary>
    [ContextMenu("Verificar Estado")]
    public void VerificarEstado()
    {
        Debug.Log("=== VERIFICACIÓN DEL ESTADO ===");
        
        PlayerController playerController = FindObjectOfType<PlayerController>();
        if (playerController != null)
        {
            Debug.Log($"PlayerController: {playerController.name}");
            Debug.Log($"  - SimpleJoystickInput: {playerController.GetComponent<SimpleJoystickInput>() != null}");
            Debug.Log($"  - KeyboardPlayerInput: {playerController.GetComponent<KeyboardPlayerInput>() != null}");
            Debug.Log($"  - SimpleMovementController: {playerController.GetComponent<SimpleMovementController>() != null}");
        }
        else
        {
            Debug.LogError("❌ No se encontró PlayerController");
        }
        
        VirtualJoystick joystick = FindObjectOfType<VirtualJoystick>();
        if (joystick != null)
        {
            Debug.Log($"VirtualJoystick: {joystick.name}");
            Debug.Log($"  - Activo: {joystick.gameObject.activeInHierarchy}");
        }
        else
        {
            Debug.LogWarning("⚠️ No se encontró VirtualJoystick");
        }
        
        Debug.Log("=== FIN DE VERIFICACIÓN ===");
    }
}
