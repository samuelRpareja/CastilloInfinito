using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Creador automático de Canvas y sistema de joystick
/// Se ejecuta automáticamente al iniciar la escena
/// </summary>
public class AutoCanvasCreator : MonoBehaviour
{
    [Header("Configuración Automática")]
    [SerializeField] private bool crearCanvasAutomaticamente = true;
    [SerializeField] private bool crearJoystickAutomaticamente = true;
    [SerializeField] private bool soloEnAndroid = true;
    
    [Header("Configuración del Canvas")]
    [SerializeField] private Vector2 referenceResolution = new Vector2(1920, 1080);
    [SerializeField] private CanvasScaler.ScaleMode scaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
    
    [Header("Configuración del Joystick")]
    [SerializeField] private Vector2 joystickPosition = new Vector2(-200, -200);
    [SerializeField] private float joystickSize = 150f;
    [SerializeField] private Color backgroundColor = new Color(1f, 1f, 1f, 0.3f);
    [SerializeField] private Color handleColor = new Color(1f, 1f, 1f, 0.8f);
    
    private Canvas canvas;
    private VirtualJoystick virtualJoystick;
    
    private void Awake()
    {
        // Solo ejecutar en Android si está configurado así
        #if !UNITY_ANDROID
        if (soloEnAndroid)
        {
            Debug.Log("AutoCanvasCreator: Solo se ejecuta en Android, saltando creación automática");
            return;
        }
        #endif
        
        if (crearCanvasAutomaticamente)
        {
            CrearCanvasCompleto();
        }
    }
    
    /// <summary>
    /// Crea el Canvas completo con joystick automáticamente
    /// </summary>
    [ContextMenu("Crear Canvas y Joystick")]
    public void CrearCanvasCompleto()
    {
        Debug.Log("AutoCanvasCreator: Iniciando creación automática de Canvas y Joystick...");
        
        // 1. Crear Canvas
        CrearCanvas();
        
        // 2. Crear Joystick
        if (crearJoystickAutomaticamente)
        {
            CrearJoystick();
        }
        
        // 3. Configurar PlayerController
        ConfigurarPlayerController();
        
        Debug.Log("AutoCanvasCreator: Canvas y Joystick creados exitosamente");
    }
    
    private void CrearCanvas()
    {
        // Verificar si ya existe un Canvas
        Canvas existingCanvas = FindObjectOfType<Canvas>();
        if (existingCanvas != null)
        {
            Debug.Log("AutoCanvasCreator: Ya existe un Canvas en la escena, usando el existente");
            canvas = existingCanvas;
            return;
        }
        
        // Crear el GameObject del Canvas
        GameObject canvasObject = new GameObject("Canvas");
        canvasObject.transform.SetParent(transform, false);
        
        // Agregar componente Canvas
        canvas = canvasObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 0;
        
        // Agregar CanvasScaler
        CanvasScaler canvasScaler = canvasObject.AddComponent<CanvasScaler>();
        canvasScaler.uiScaleMode = scaleMode;
        canvasScaler.referenceResolution = referenceResolution;
        canvasScaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
        canvasScaler.matchWidthOrHeight = 0.5f;
        
        // Agregar GraphicRaycaster
        canvasObject.AddComponent<GraphicRaycaster>();
        
        // Crear EventSystem si no existe
        if (FindObjectOfType<UnityEngine.EventSystems.EventSystem>() == null)
        {
            GameObject eventSystemObject = new GameObject("EventSystem");
            eventSystemObject.AddComponent<UnityEngine.EventSystems.EventSystem>();
            eventSystemObject.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
        }
        
        Debug.Log("AutoCanvasCreator: Canvas creado correctamente");
    }
    
    private void CrearJoystick()
    {
        if (canvas == null)
        {
            Debug.LogError("AutoCanvasCreator: No se puede crear el joystick sin Canvas");
            return;
        }
        
        // Crear el contenedor del joystick
        GameObject joystickContainer = new GameObject("VirtualJoystick");
        joystickContainer.transform.SetParent(canvas.transform, false);
        
        // Configurar RectTransform del contenedor
        RectTransform containerRect = joystickContainer.AddComponent<RectTransform>();
        containerRect.anchorMin = new Vector2(0, 0);
        containerRect.anchorMax = new Vector2(0, 0);
        containerRect.anchoredPosition = joystickPosition;
        containerRect.sizeDelta = new Vector2(joystickSize, joystickSize);
        
        // Crear el fondo del joystick
        GameObject background = new GameObject("Background");
        background.transform.SetParent(joystickContainer.transform, false);
        
        RectTransform backgroundRect = background.AddComponent<RectTransform>();
        backgroundRect.anchorMin = Vector2.zero;
        backgroundRect.anchorMax = Vector2.one;
        backgroundRect.offsetMin = Vector2.zero;
        backgroundRect.offsetMax = Vector2.zero;
        
        Image backgroundImage = background.AddComponent<Image>();
        backgroundImage.color = backgroundColor;
        backgroundImage.sprite = CrearSpriteCircular();
        backgroundImage.type = Image.Type.Simple;
        
        // Crear el handle del joystick
        GameObject handle = new GameObject("Handle");
        handle.transform.SetParent(background.transform, false);
        
        RectTransform handleRect = handle.AddComponent<RectTransform>();
        handleRect.anchorMin = new Vector2(0.5f, 0.5f);
        handleRect.anchorMax = new Vector2(0.5f, 0.5f);
        handleRect.anchoredPosition = Vector2.zero;
        handleRect.sizeDelta = new Vector2(joystickSize * 0.4f, joystickSize * 0.4f);
        
        Image handleImage = handle.AddComponent<Image>();
        handleImage.color = handleColor;
        handleImage.sprite = CrearSpriteCircular();
        handleImage.type = Image.Type.Simple;
        
        // Agregar el script VirtualJoystick
        virtualJoystick = joystickContainer.AddComponent<VirtualJoystick>();
        virtualJoystick.joystickBackground = backgroundRect;
        virtualJoystick.joystickHandle = handleRect;
        virtualJoystick.joystickRange = joystickSize * 0.3f;
        virtualJoystick.mostrarJoystick = true;
        virtualJoystick.sensibilidad = 1f;
        virtualJoystick.deadZone = 0.1f;
        virtualJoystick.animarHandle = true;
        virtualJoystick.animacionVelocidad = 15f;
        
        Debug.Log("AutoCanvasCreator: Joystick creado correctamente");
    }
    
    private void ConfigurarPlayerController()
    {
        // Buscar el PlayerController
        PlayerController playerController = FindObjectOfType<PlayerController>();
        if (playerController == null)
        {
            Debug.LogWarning("AutoCanvasCreator: No se encontró PlayerController en la escena");
            return;
        }
        
        // Verificar si ya tiene AdaptivePlayerInput
        AdaptivePlayerInput existingInput = playerController.GetComponent<AdaptivePlayerInput>();
        if (existingInput != null)
        {
            Debug.Log("AutoCanvasCreator: PlayerController ya tiene AdaptivePlayerInput configurado");
            return;
        }
        
        // Remover KeyboardPlayerInput si existe
        KeyboardPlayerInput oldInput = playerController.GetComponent<KeyboardPlayerInput>();
        if (oldInput != null)
        {
            DestroyImmediate(oldInput);
            Debug.Log("AutoCanvasCreator: KeyboardPlayerInput removido");
        }
        
        // Agregar AdaptivePlayerInput
        AdaptivePlayerInput adaptiveInput = playerController.gameObject.AddComponent<AdaptivePlayerInput>();
        
        // Agregar HybridInputProvider
        HybridInputProvider hybridProvider = playerController.gameObject.AddComponent<HybridInputProvider>();
        
        // Agregar JoystickInputProvider
        JoystickInputProvider joystickProvider = playerController.gameObject.AddComponent<JoystickInputProvider>();
        
        // Configurar el JoystickInputProvider con el joystick creado
        if (virtualJoystick != null)
        {
            joystickProvider.SetVirtualJoystick(virtualJoystick);
        }
        
        Debug.Log("AutoCanvasCreator: PlayerController configurado correctamente");
    }
    
    private Sprite CrearSpriteCircular()
    {
        // Crear un sprite circular simple
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
    /// Obtiene el Canvas creado
    /// </summary>
    public Canvas GetCanvas()
    {
        return canvas;
    }
    
    /// <summary>
    /// Obtiene el VirtualJoystick creado
    /// </summary>
    public VirtualJoystick GetVirtualJoystick()
    {
        return virtualJoystick;
    }
    
    /// <summary>
    /// Configura la posición del joystick
    /// </summary>
    public void SetJoystickPosition(Vector2 position)
    {
        joystickPosition = position;
        if (virtualJoystick != null)
        {
            RectTransform rect = virtualJoystick.GetComponent<RectTransform>();
            rect.anchoredPosition = position;
        }
    }
    
    /// <summary>
    /// Configura el tamaño del joystick
    /// </summary>
    public void SetJoystickSize(float size)
    {
        joystickSize = size;
        if (virtualJoystick != null)
        {
            RectTransform rect = virtualJoystick.GetComponent<RectTransform>();
            rect.sizeDelta = new Vector2(size, size);
            virtualJoystick.joystickRange = size * 0.3f;
        }
    }
}
