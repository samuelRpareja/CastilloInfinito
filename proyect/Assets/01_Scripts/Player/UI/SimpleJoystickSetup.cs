using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Configurador súper simple del joystick para Android
/// Solo agrega este script a cualquier GameObject en tu escena
/// </summary>
public class SimpleJoystickSetup : MonoBehaviour
{
    [Header("Configuración Rápida")]
    [SerializeField] private bool crearAutomaticamente = true;
    [SerializeField] private Vector2 posicionJoystick = new Vector2(-200, -200);
    [SerializeField] private float tamanoJoystick = 150f;
    
    private void Start()
    {
        if (crearAutomaticamente)
        {
            CrearJoystickCompleto();
        }
    }
    
    /// <summary>
    /// Crea todo el sistema de joystick automáticamente
    /// </summary>
    [ContextMenu("Crear Joystick Completo")]
    public void CrearJoystickCompleto()
    {
        Debug.Log("SimpleJoystickSetup: Creando joystick automáticamente...");
        
        // 1. Crear Canvas si no existe
        Canvas canvas = CrearOConsultarCanvas();
        
        // 2. Crear Joystick
        CrearJoystick(canvas);
        
        // 3. Configurar PlayerController
        ConfigurarPlayerController();
        
        Debug.Log("SimpleJoystickSetup: ¡Joystick creado exitosamente!");
    }
    
    private Canvas CrearOConsultarCanvas()
    {
        // Buscar Canvas existente
        Canvas canvas = FindObjectOfType<Canvas>();
        
        if (canvas == null)
        {
            Debug.Log("SimpleJoystickSetup: Creando nuevo Canvas...");
            
            // Crear Canvas
            GameObject canvasObject = new GameObject("Canvas");
            canvas = canvasObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            
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
                GameObject eventSystem = new GameObject("EventSystem");
                eventSystem.AddComponent<UnityEngine.EventSystems.EventSystem>();
                eventSystem.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
            }
        }
        else
        {
            Debug.Log("SimpleJoystickSetup: Usando Canvas existente");
        }
        
        return canvas;
    }
    
    private void CrearJoystick(Canvas canvas)
    {
        // Crear contenedor del joystick
        GameObject joystickObject = new GameObject("VirtualJoystick");
        joystickObject.transform.SetParent(canvas.transform, false);
        
        // Configurar posición y tamaño
        RectTransform joystickRect = joystickObject.AddComponent<RectTransform>();
        joystickRect.anchorMin = new Vector2(0, 0);
        joystickRect.anchorMax = new Vector2(0, 0);
        joystickRect.anchoredPosition = posicionJoystick;
        joystickRect.sizeDelta = new Vector2(tamanoJoystick, tamanoJoystick);
        
        // Crear fondo
        GameObject background = new GameObject("Background");
        background.transform.SetParent(joystickObject.transform, false);
        
        RectTransform bgRect = background.AddComponent<RectTransform>();
        bgRect.anchorMin = Vector2.zero;
        bgRect.anchorMax = Vector2.one;
        bgRect.offsetMin = Vector2.zero;
        bgRect.offsetMax = Vector2.zero;
        
        Image bgImage = background.AddComponent<Image>();
        bgImage.color = new Color(1f, 1f, 1f, 0.3f);
        bgImage.sprite = CrearSpriteCircular();
        
        // Crear handle
        GameObject handle = new GameObject("Handle");
        handle.transform.SetParent(background.transform, false);
        
        RectTransform handleRect = handle.AddComponent<RectTransform>();
        handleRect.anchorMin = new Vector2(0.5f, 0.5f);
        handleRect.anchorMax = new Vector2(0.5f, 0.5f);
        handleRect.anchoredPosition = Vector2.zero;
        handleRect.sizeDelta = new Vector2(tamanoJoystick * 0.4f, tamanoJoystick * 0.4f);
        
        Image handleImage = handle.AddComponent<Image>();
        handleImage.color = new Color(1f, 1f, 1f, 0.8f);
        handleImage.sprite = CrearSpriteCircular();
        
        // Agregar VirtualJoystick
        VirtualJoystick virtualJoystick = joystickObject.AddComponent<VirtualJoystick>();
        virtualJoystick.joystickBackground = bgRect;
        virtualJoystick.joystickHandle = handleRect;
        virtualJoystick.joystickRange = tamanoJoystick * 0.3f;
        virtualJoystick.mostrarJoystick = true;
        virtualJoystick.sensibilidad = 1f;
        virtualJoystick.deadZone = 0.1f;
        virtualJoystick.animarHandle = true;
        virtualJoystick.animacionVelocidad = 15f;
        
        Debug.Log("SimpleJoystickSetup: Joystick creado correctamente");
    }
    
    private void ConfigurarPlayerController()
    {
        // Buscar PlayerController
        PlayerController playerController = FindObjectOfType<PlayerController>();
        if (playerController == null)
        {
            Debug.LogWarning("SimpleJoystickSetup: No se encontró PlayerController");
            return;
        }
        
        // Verificar si ya está configurado
        if (playerController.GetComponent<AdaptivePlayerInput>() != null)
        {
            Debug.Log("SimpleJoystickSetup: PlayerController ya está configurado");
            return;
        }
        
        // Remover input anterior
        KeyboardPlayerInput oldInput = playerController.GetComponent<KeyboardPlayerInput>();
        if (oldInput != null)
        {
            DestroyImmediate(oldInput);
        }
        
        // Agregar nuevos componentes
        playerController.gameObject.AddComponent<AdaptivePlayerInput>();
        playerController.gameObject.AddComponent<HybridInputProvider>();
        playerController.gameObject.AddComponent<JoystickInputProvider>();
        
        Debug.Log("SimpleJoystickSetup: PlayerController configurado correctamente");
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
}
