using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Script que garantiza que el joystick se muestre correctamente en Android
/// </summary>
public class AndroidJoystickFix : MonoBehaviour
{
    [Header("Configuración")]
    [SerializeField] private bool crearJoystickSiNoExiste = true;
    [SerializeField] private bool forzarVisibilidad = true;
    [SerializeField] private Vector2 posicionJoystick = new Vector2(-200, -200);
    [SerializeField] private float tamanoJoystick = 150f;
    
    private void Start()
    {
        // Ejecutar en el primer frame para asegurar que todo esté inicializado
        Invoke(nameof(ConfigurarJoystickAndroid), 0.1f);
    }
    
    private void ConfigurarJoystickAndroid()
    {
        Debug.Log("AndroidJoystickFix: Configurando joystick para Android...");
        
        // Verificar si estamos en Android
        #if UNITY_ANDROID
        Debug.Log("AndroidJoystickFix: Ejecutándose en Android");
        #else
        Debug.Log("AndroidJoystickFix: NO ejecutándose en Android, saltando configuración");
        return;
        #endif
        
        // 1. Verificar/Crear Canvas
        Canvas canvas = VerificarOCrearCanvas();
        
        // 2. Verificar/Crear Joystick
        VirtualJoystick joystick = VerificarOCrearJoystick(canvas);
        
        // 3. Forzar visibilidad
        if (forzarVisibilidad && joystick != null)
        {
            ForzarVisibilidadJoystick(joystick);
        }
        
        // 4. Configurar PlayerController
        ConfigurarPlayerController();
        
        Debug.Log("AndroidJoystickFix: Configuración completada");
    }
    
    private Canvas VerificarOCrearCanvas()
    {
        Canvas canvas = FindObjectOfType<Canvas>();
        
        if (canvas == null)
        {
            Debug.Log("AndroidJoystickFix: Creando Canvas...");
            
            // Crear Canvas
            GameObject canvasObject = new GameObject("Canvas_Android");
            canvas = canvasObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 100; // Alto para estar encima de todo
            
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
                GameObject eventSystem = new GameObject("EventSystem_Android");
                eventSystem.AddComponent<UnityEngine.EventSystems.EventSystem>();
                eventSystem.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
            }
        }
        else
        {
            Debug.Log("AndroidJoystickFix: Canvas existente encontrado");
        }
        
        return canvas;
    }
    
    private VirtualJoystick VerificarOCrearJoystick(Canvas canvas)
    {
        VirtualJoystick joystick = FindObjectOfType<VirtualJoystick>();
        
        if (joystick == null && crearJoystickSiNoExiste)
        {
            Debug.Log("AndroidJoystickFix: Creando VirtualJoystick...");
            
            // Crear contenedor del joystick
            GameObject joystickObject = new GameObject("VirtualJoystick_Android");
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
            bgImage.color = new Color(1f, 1f, 1f, 0.4f); // Más visible
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
            handleImage.color = new Color(1f, 1f, 1f, 0.9f); // Más visible
            handleImage.sprite = CrearSpriteCircular();
            
            // Agregar VirtualJoystick
            joystick = joystickObject.AddComponent<VirtualJoystick>();
            joystick.joystickBackground = bgRect;
            joystick.joystickHandle = handleRect;
            joystick.joystickRange = tamanoJoystick * 0.3f;
            joystick.mostrarJoystick = true;
            joystick.sensibilidad = 1f;
            joystick.deadZone = 0.1f;
            joystick.animarHandle = true;
            joystick.animacionVelocidad = 15f;
        }
        else if (joystick != null)
        {
            Debug.Log("AndroidJoystickFix: VirtualJoystick existente encontrado");
        }
        
        return joystick;
    }
    
    private void ForzarVisibilidadJoystick(VirtualJoystick joystick)
    {
        if (joystick == null) return;
        
        Debug.Log("AndroidJoystickFix: Forzando visibilidad del joystick...");
        
        // Forzar que el joystick sea visible
        joystick.gameObject.SetActive(true);
        joystick.mostrarJoystick = true;
        
        // Forzar que el fondo sea visible
        if (joystick.joystickBackground != null)
        {
            joystick.joystickBackground.gameObject.SetActive(true);
            Image bgImage = joystick.joystickBackground.GetComponent<Image>();
            if (bgImage != null)
            {
                bgImage.color = new Color(1f, 1f, 1f, 0.5f); // Más visible
            }
        }
        
        // Forzar que el handle sea visible
        if (joystick.joystickHandle != null)
        {
            joystick.joystickHandle.gameObject.SetActive(true);
            Image handleImage = joystick.joystickHandle.GetComponent<Image>();
            if (handleImage != null)
            {
                handleImage.color = new Color(1f, 1f, 1f, 1f); // Completamente opaco
            }
        }
        
        Debug.Log("AndroidJoystickFix: Visibilidad forzada");
    }
    
    private void ConfigurarPlayerController()
    {
        PlayerController playerController = FindObjectOfType<PlayerController>();
        if (playerController == null)
        {
            Debug.LogWarning("AndroidJoystickFix: No se encontró PlayerController");
            return;
        }
        
        // Verificar si ya está configurado
        if (playerController.GetComponent<AdaptivePlayerInput>() != null)
        {
            Debug.Log("AndroidJoystickFix: PlayerController ya está configurado");
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
        
        Debug.Log("AndroidJoystickFix: PlayerController configurado");
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
    /// Método para ejecutar la configuración manualmente
    /// </summary>
    [ContextMenu("Configurar Joystick Android")]
    public void ConfigurarJoystickAndroidManual()
    {
        ConfigurarJoystickAndroid();
    }
}
