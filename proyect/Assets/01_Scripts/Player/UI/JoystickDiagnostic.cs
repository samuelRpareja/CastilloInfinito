using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Script de diagnóstico para verificar por qué no se visualiza el joystick en Android
/// </summary>
public class JoystickDiagnostic : MonoBehaviour
{
    [Header("Diagnóstico")]
    [SerializeField] private bool mostrarDiagnostico = true;
    [SerializeField] private bool forzarJoystickVisible = true;
    
    private void Start()
    {
        if (mostrarDiagnostico)
        {
            DiagnosticarJoystick();
        }
    }
    
    private void DiagnosticarJoystick()
    {
        Debug.Log("=== DIAGNÓSTICO DEL JOYSTICK ===");
        
        // 1. Verificar plataforma
        Debug.Log($"Plataforma actual: {Application.platform}");
        Debug.Log($"Es Android: {Application.platform == RuntimePlatform.Android}");
        
        // 2. Verificar Canvas
        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas != null)
        {
            Debug.Log($"Canvas encontrado: {canvas.name}");
            Debug.Log($"Canvas activo: {canvas.gameObject.activeInHierarchy}");
            Debug.Log($"Render Mode: {canvas.renderMode}");
            Debug.Log($"Sorting Order: {canvas.sortingOrder}");
        }
        else
        {
            Debug.LogError("❌ NO SE ENCONTRÓ CANVAS EN LA ESCENA");
        }
        
        // 3. Verificar VirtualJoystick
        VirtualJoystick joystick = FindObjectOfType<VirtualJoystick>();
        if (joystick != null)
        {
            Debug.Log($"VirtualJoystick encontrado: {joystick.name}");
            Debug.Log($"Joystick activo: {joystick.gameObject.activeInHierarchy}");
            Debug.Log($"Mostrar Joystick: {joystick.mostrarJoystick}");
            Debug.Log($"Joystick Background: {joystick.joystickBackground != null}");
            Debug.Log($"Joystick Handle: {joystick.joystickHandle != null}");
            
            if (joystick.joystickBackground != null)
            {
                Debug.Log($"Background activo: {joystick.joystickBackground.gameObject.activeInHierarchy}");
                Debug.Log($"Background visible: {joystick.joystickBackground.gameObject.activeSelf}");
            }
            
            if (joystick.joystickHandle != null)
            {
                Debug.Log($"Handle activo: {joystick.joystickHandle.gameObject.activeInHierarchy}");
                Debug.Log($"Handle visible: {joystick.joystickHandle.gameObject.activeSelf}");
            }
        }
        else
        {
            Debug.LogError("❌ NO SE ENCONTRÓ VIRTUALJOYSTICK EN LA ESCENA");
        }
        
        // 4. Verificar PlayerController
        PlayerController playerController = FindObjectOfType<PlayerController>();
        if (playerController != null)
        {
            Debug.Log($"PlayerController encontrado: {playerController.name}");
            Debug.Log($"AdaptivePlayerInput: {playerController.GetComponent<AdaptivePlayerInput>() != null}");
            Debug.Log($"KeyboardPlayerInput: {playerController.GetComponent<KeyboardPlayerInput>() != null}");
        }
        else
        {
            Debug.LogError("❌ NO SE ENCONTRÓ PLAYERCONTROLLER EN LA ESCENA");
        }
        
        // 5. Verificar EventSystem
        UnityEngine.EventSystems.EventSystem eventSystem = FindObjectOfType<UnityEngine.EventSystems.EventSystem>();
        if (eventSystem != null)
        {
            Debug.Log($"EventSystem encontrado: {eventSystem.name}");
        }
        else
        {
            Debug.LogWarning("⚠️ NO SE ENCONTRÓ EVENTSYSTEM EN LA ESCENA");
        }
        
        Debug.Log("=== FIN DEL DIAGNÓSTICO ===");
        
        // 6. Intentar corregir problemas
        if (forzarJoystickVisible)
        {
            CorregirProblemasJoystick();
        }
    }
    
    private void CorregirProblemasJoystick()
    {
        Debug.Log("=== INTENTANDO CORREGIR PROBLEMAS ===");
        
        // Corregir VirtualJoystick
        VirtualJoystick joystick = FindObjectOfType<VirtualJoystick>();
        if (joystick != null)
        {
            // Forzar que sea visible
            joystick.gameObject.SetActive(true);
            joystick.mostrarJoystick = true;
            
            if (joystick.joystickBackground != null)
            {
                joystick.joystickBackground.gameObject.SetActive(true);
            }
            
            if (joystick.joystickHandle != null)
            {
                joystick.joystickHandle.gameObject.SetActive(true);
            }
            
            Debug.Log("✅ VirtualJoystick corregido");
        }
        
        // Corregir Canvas
        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas != null)
        {
            canvas.gameObject.SetActive(true);
            canvas.enabled = true;
            Debug.Log("✅ Canvas corregido");
        }
        
        Debug.Log("=== CORRECCIONES APLICADAS ===");
    }
    
    /// <summary>
    /// Método para ejecutar diagnóstico manualmente
    /// </summary>
    [ContextMenu("Ejecutar Diagnóstico")]
    public void EjecutarDiagnostico()
    {
        DiagnosticarJoystick();
    }
    
    /// <summary>
    /// Método para forzar visibilidad del joystick
    /// </summary>
    [ContextMenu("Forzar Joystick Visible")]
    public void ForzarJoystickVisible()
    {
        VirtualJoystick joystick = FindObjectOfType<VirtualJoystick>();
        if (joystick != null)
        {
            joystick.gameObject.SetActive(true);
            joystick.mostrarJoystick = true;
            
            if (joystick.joystickBackground != null)
            {
                joystick.joystickBackground.gameObject.SetActive(true);
                Image bgImage = joystick.joystickBackground.GetComponent<Image>();
                if (bgImage != null)
                {
                    bgImage.color = new Color(1f, 1f, 1f, 0.5f); // Hacer más visible
                }
            }
            
            if (joystick.joystickHandle != null)
            {
                joystick.joystickHandle.gameObject.SetActive(true);
                Image handleImage = joystick.joystickHandle.GetComponent<Image>();
                if (handleImage != null)
                {
                    handleImage.color = new Color(1f, 1f, 1f, 1f); // Completamente opaco
                }
            }
            
            Debug.Log("✅ Joystick forzado a ser visible");
        }
        else
        {
            Debug.LogError("❌ No se encontró VirtualJoystick para forzar visibilidad");
        }
    }
}
