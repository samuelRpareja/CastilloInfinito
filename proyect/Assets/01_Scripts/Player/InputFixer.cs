using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Script que corrige automáticamente todos los problemas de input
/// </summary>
public class InputFixer : MonoBehaviour
{
    [Header("Configuración")]
    [SerializeField] private bool corregirAutomaticamente = true;
    [SerializeField] private bool crearJoystickSiNoExiste = true;
    
    private void Start()
    {
        if (corregirAutomaticamente)
        {
            // Ejecutar después de un frame para asegurar que todo esté inicializado
            Invoke(nameof(CorregirSistemaCompleto), 0.1f);
        }
    }
    
    /// <summary>
    /// Corrige todo el sistema de input
    /// </summary>
    [ContextMenu("Corregir Sistema Completo")]
    public void CorregirSistemaCompleto()
    {
        Debug.Log("InputFixer: Iniciando corrección completa del sistema...");
        
        // 1. Verificar/Crear Canvas
        Canvas canvas = VerificarOCrearCanvas();
        
        // 2. Verificar/Crear Joystick
        VirtualJoystick joystick = VerificarOCrearJoystick(canvas);
        
        // 3. Corregir PlayerController
        CorregirPlayerController(joystick);
        
        Debug.Log("InputFixer: Corrección completada");
    }
    
    private Canvas VerificarOCrearCanvas()
    {
        Canvas canvas = FindObjectOfType<Canvas>();
        
        if (canvas == null)
        {
            Debug.Log("InputFixer: Creando Canvas...");
            
            // Crear Canvas
            GameObject canvasObject = new GameObject("Canvas_InputFixer");
            canvas = canvasObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 100;
            
            // Agregar CanvasScaler
            CanvasScaler scaler = canvasObject.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            scaler.matchWidthOrHeight = 0.5f;
            
            // Agregar GraphicRaycaster
            canvasObject.AddComponent<GraphicRaycaster>();
            
            // Crear EventSystem si no existe
            if (FindObjectOfType<UnityEngine.EventSystems.EventSystem>() == null)
            {
                GameObject eventSystem = new GameObject("EventSystem_InputFixer");
                eventSystem.AddComponent<UnityEngine.EventSystems.EventSystem>();
                eventSystem.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
            }
        }
        else
        {
            Debug.Log("InputFixer: Canvas existente encontrado");
        }
        
        return canvas;
    }
    
    private VirtualJoystick VerificarOCrearJoystick(Canvas canvas)
    {
        VirtualJoystick joystick = FindObjectOfType<VirtualJoystick>();
        
        if (joystick == null && crearJoystickSiNoExiste)
        {
            Debug.Log("InputFixer: Creando VirtualJoystick...");
            
            // Crear contenedor del joystick
            GameObject joystickObject = new GameObject("VirtualJoystick_InputFixer");
            joystickObject.transform.SetParent(canvas.transform, false);
            
            // Configurar posición y tamaño
            RectTransform joystickRect = joystickObject.AddComponent<RectTransform>();
            joystickRect.anchorMin = new Vector2(0, 0);
            joystickRect.anchorMax = new Vector2(0, 0);
            joystickRect.anchoredPosition = new Vector2(-200, -200);
            joystickRect.sizeDelta = new Vector2(150, 150);
            
            // Crear fondo
            GameObject background = new GameObject("Background");
            background.transform.SetParent(joystickObject.transform, false);
            
            RectTransform bgRect = background.AddComponent<RectTransform>();
            bgRect.anchorMin = Vector2.zero;
            bgRect.anchorMax = Vector2.one;
            bgRect.offsetMin = Vector2.zero;
            bgRect.offsetMax = Vector2.zero;
            
            Image bgImage = background.AddComponent<Image>();
            bgImage.color = new Color(1f, 1f, 1f, 0.4f);
            bgImage.sprite = CrearSpriteCircular();
            
            // Crear handle
            GameObject handle = new GameObject("Handle");
            handle.transform.SetParent(background.transform, false);
            
            RectTransform handleRect = handle.AddComponent<RectTransform>();
            handleRect.anchorMin = new Vector2(0.5f, 0.5f);
            handleRect.anchorMax = new Vector2(0.5f, 0.5f);
            handleRect.anchoredPosition = Vector2.zero;
            handleRect.sizeDelta = new Vector2(60, 60);
            
            Image handleImage = handle.AddComponent<Image>();
            handleImage.color = new Color(1f, 1f, 1f, 0.9f);
            handleImage.sprite = CrearSpriteCircular();
            
            // Agregar VirtualJoystick
            joystick = joystickObject.AddComponent<VirtualJoystick>();
            joystick.joystickBackground = bgRect;
            joystick.joystickHandle = handleRect;
            joystick.joystickRange = 45f;
            joystick.mostrarJoystick = true;
            joystick.sensibilidad = 1f;
            joystick.deadZone = 0.1f;
            joystick.animarHandle = true;
            joystick.animacionVelocidad = 15f;
        }
        else if (joystick != null)
        {
            Debug.Log("InputFixer: VirtualJoystick existente encontrado");
        }
        
        return joystick;
    }
    
    private void CorregirPlayerController(VirtualJoystick joystick)
    {
        PlayerController playerController = FindObjectOfType<PlayerController>();
        if (playerController == null)
        {
            Debug.LogError("InputFixer: No se encontró PlayerController");
            return;
        }
        
        Debug.Log($"InputFixer: Corrigiendo PlayerController - {playerController.name}");
        
        // Remover todos los componentes de input existentes
        KeyboardPlayerInput keyboardInput = playerController.GetComponent<KeyboardPlayerInput>();
        if (keyboardInput != null)
        {
            DestroyImmediate(keyboardInput);
            Debug.Log("InputFixer: KeyboardPlayerInput removido");
        }
        
        SimpleJoystickInput joystickInput = playerController.GetComponent<SimpleJoystickInput>();
        if (joystickInput != null)
        {
            DestroyImmediate(joystickInput);
            Debug.Log("InputFixer: SimpleJoystickInput existente removido");
        }
        
        // Agregar nuevo SimpleJoystickInput
        joystickInput = playerController.gameObject.AddComponent<SimpleJoystickInput>();
        
        // Configurar el joystick
        if (joystick != null)
        {
            joystickInput.SetJoystick(joystick);
            Debug.Log("InputFixer: Joystick configurado");
        }
        
        // Verificar que SimpleMovementController existe
        SimpleMovementController movementController = playerController.GetComponent<SimpleMovementController>();
        if (movementController == null)
        {
            Debug.LogError("InputFixer: No se encontró SimpleMovementController");
        }
        else
        {
            Debug.Log($"InputFixer: SimpleMovementController encontrado - Velocidad: {movementController.velocidadMovimiento}");
        }
        
        Debug.Log("InputFixer: PlayerController corregido exitosamente");
    }
    
    private Sprite CrearSpriteCircular()
    {
        Texture2D texture = new Texture2D(64, 64);
        Color[] pixels = new Color[64 * 64];
        
        Vector2 center = new Vector2(32, 32);
        float radius = 30f;
        
        for (int x = 0; x < 64; x++)
        {
            for (int y = 0; y < 64; y++)
            {
                Vector2 pos = new Vector2(x, y);
                float distance = Vector2.Distance(pos, center);
                
                if (distance <= radius)
                {
                    pixels[y * 64 + x] = Color.white;
                }
                else
                {
                    pixels[y * 64 + x] = Color.clear;
                }
            }
        }
        
        texture.SetPixels(pixels);
        texture.Apply();
        
        return Sprite.Create(texture, new Rect(0, 0, 64, 64), new Vector2(0.5f, 0.5f));
    }
    
    /// <summary>
    /// Método para verificar el estado actual
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
        
        VirtualJoystick joystick = FindObjectOfType<VirtualJoystick>();
        if (joystick != null)
        {
            Debug.Log($"VirtualJoystick: {joystick.name}");
            Debug.Log($"  - Activo: {joystick.gameObject.activeInHierarchy}");
        }
        
        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas != null)
        {
            Debug.Log($"Canvas: {canvas.name}");
            Debug.Log($"  - Activo: {canvas.gameObject.activeInHierarchy}");
        }
        
        Debug.Log("=== FIN DE VERIFICACIÓN ===");
    }
}
