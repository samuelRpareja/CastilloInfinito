using UnityEngine;

/// <summary>
/// Script que reemplaza de forma segura KeyboardPlayerInput con SimpleJoystickInput
/// Maneja las dependencias del PlayerController correctamente
/// </summary>
public class SafeInputReplacer : MonoBehaviour
{
    [Header("Configuración")]
    [SerializeField] private bool reemplazarAutomaticamente = true;
    [SerializeField] private bool forzarReemplazo = false;
    
    private void Start()
    {
        if (reemplazarAutomaticamente)
        {
            // Ejecutar después de un frame para asegurar que todo esté inicializado
            Invoke(nameof(ReemplazarInputSeguro), 0.1f);
        }
    }
    
    /// <summary>
    /// Reemplaza de forma segura el input del player
    /// </summary>
    [ContextMenu("Reemplazar Input Seguro")]
    public void ReemplazarInputSeguro()
    {
        Debug.Log("SafeInputReplacer: Iniciando reemplazo seguro de input...");
        
        // Buscar PlayerController
        PlayerController playerController = FindObjectOfType<PlayerController>();
        if (playerController == null)
        {
            Debug.LogError("SafeInputReplacer: No se encontró PlayerController");
            return;
        }
        
        Debug.Log($"SafeInputReplacer: PlayerController encontrado - {playerController.name}");
        
        // Verificar si ya tiene SimpleJoystickInput
        SimpleJoystickInput existingInput = playerController.GetComponent<SimpleJoystickInput>();
        if (existingInput != null)
        {
            Debug.Log("SafeInputReplacer: SimpleJoystickInput ya existe");
            return;
        }
        
        // Verificar si tiene KeyboardPlayerInput
        KeyboardPlayerInput oldInput = playerController.GetComponent<KeyboardPlayerInput>();
        if (oldInput != null)
        {
            Debug.Log("SafeInputReplacer: KeyboardPlayerInput encontrado, reemplazando...");
            
            // Desactivar temporalmente el componente
            oldInput.enabled = false;
            
            // Agregar SimpleJoystickInput
            SimpleJoystickInput newInput = playerController.gameObject.AddComponent<SimpleJoystickInput>();
            
            // Buscar y configurar el joystick
            VirtualJoystick joystick = FindObjectOfType<VirtualJoystick>();
            if (joystick != null)
            {
                newInput.SetJoystick(joystick);
                Debug.Log("SafeInputReplacer: Joystick configurado");
            }
            else
            {
                Debug.LogWarning("SafeInputReplacer: No se encontró VirtualJoystick");
            }
            
            // Remover el componente anterior después de un frame
            StartCoroutine(RemoverComponenteDespues(oldInput));
            
            Debug.Log("SafeInputReplacer: Reemplazo iniciado exitosamente");
        }
        else
        {
            Debug.Log("SafeInputReplacer: No se encontró KeyboardPlayerInput, agregando SimpleJoystickInput...");
            
            // Agregar SimpleJoystickInput directamente
            SimpleJoystickInput newInput = playerController.gameObject.AddComponent<SimpleJoystickInput>();
            
            // Buscar y configurar el joystick
            VirtualJoystick joystick = FindObjectOfType<VirtualJoystick>();
            if (joystick != null)
            {
                newInput.SetJoystick(joystick);
                Debug.Log("SafeInputReplacer: Joystick configurado");
            }
            
            Debug.Log("SafeInputReplacer: SimpleJoystickInput agregado exitosamente");
        }
    }
    
    private System.Collections.IEnumerator RemoverComponenteDespues(KeyboardPlayerInput oldInput)
    {
        // Esperar un frame
        yield return null;
        
        // Remover el componente anterior
        if (oldInput != null)
        {
            DestroyImmediate(oldInput);
            Debug.Log("SafeInputReplacer: KeyboardPlayerInput removido exitosamente");
        }
    }
    
    /// <summary>
    /// Verifica el estado actual del input
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
            Debug.Log($"  - AnimatorDriver: {playerController.GetComponent<AnimatorDriver>() != null}");
            Debug.Log($"  - SimpleAttacker: {playerController.GetComponent<SimpleAttacker>() != null}");
            Debug.Log($"  - Health: {playerController.GetComponent<Health>() != null}");
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
            Debug.Log($"  - Mostrar Joystick: {joystick.mostrarJoystick}");
        }
        else
        {
            Debug.LogWarning("⚠️ No se encontró VirtualJoystick");
        }
        
        Debug.Log("=== FIN DE VERIFICACIÓN ===");
    }
    
    /// <summary>
    /// Fuerza el reemplazo incluso si hay dependencias
    /// </summary>
    [ContextMenu("Forzar Reemplazo")]
    public void ForzarReemplazo()
    {
        Debug.Log("SafeInputReplacer: Forzando reemplazo...");
        
        PlayerController playerController = FindObjectOfType<PlayerController>();
        if (playerController == null)
        {
            Debug.LogError("SafeInputReplacer: No se encontró PlayerController");
            return;
        }
        
        // Remover todos los componentes de input
        KeyboardPlayerInput keyboardInput = playerController.GetComponent<KeyboardPlayerInput>();
        if (keyboardInput != null)
        {
            DestroyImmediate(keyboardInput);
            Debug.Log("SafeInputReplacer: KeyboardPlayerInput forzado a remover");
        }
        
        SimpleJoystickInput joystickInput = playerController.GetComponent<SimpleJoystickInput>();
        if (joystickInput != null)
        {
            DestroyImmediate(joystickInput);
            Debug.Log("SafeInputReplacer: SimpleJoystickInput existente removido");
        }
        
        // Agregar nuevo SimpleJoystickInput
        SimpleJoystickInput newInput = playerController.gameObject.AddComponent<SimpleJoystickInput>();
        
        // Configurar joystick
        VirtualJoystick joystick = FindObjectOfType<VirtualJoystick>();
        if (joystick != null)
        {
            newInput.SetJoystick(joystick);
        }
        
        Debug.Log("SafeInputReplacer: Reemplazo forzado completado");
    }
}
