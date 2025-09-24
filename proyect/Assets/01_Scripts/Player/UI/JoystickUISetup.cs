using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Configurador automático de la UI del joystick virtual
/// Crea y configura los elementos necesarios para el joystick
/// </summary>
public class JoystickUISetup : MonoBehaviour
{
    [Header("Configuración del Joystick")]
    [SerializeField] private Canvas targetCanvas;
    [SerializeField] private Vector2 joystickPosition = new Vector2(-200, -200);
    [SerializeField] private float joystickSize = 150f;
    [SerializeField] private Color backgroundColor = new Color(1f, 1f, 1f, 0.3f);
    [SerializeField] private Color handleColor = new Color(1f, 1f, 1f, 0.8f);
    
    [Header("Referencias")]
    [SerializeField] private VirtualJoystick virtualJoystick;
    
    private GameObject joystickUI;
    private RectTransform joystickBackground;
    private RectTransform joystickHandle;
    
    private void Awake()
    {
        SetupJoystickUI();
    }
    
    private void SetupJoystickUI()
    {
        #if UNITY_ANDROID
        CreateJoystickUI();
        ConfigureJoystick();
        #else
        // En otras plataformas, ocultar el joystick
        if (virtualJoystick != null)
        {
            virtualJoystick.gameObject.SetActive(false);
        }
        #endif
    }
    
    private void CreateJoystickUI()
    {
        // Buscar el canvas si no está asignado
        if (targetCanvas == null)
        {
            targetCanvas = FindObjectOfType<Canvas>();
            if (targetCanvas == null)
            {
                Debug.LogError("JoystickUISetup: No se encontró un Canvas en la escena");
                return;
            }
        }
        
        // Crear el contenedor del joystick
        joystickUI = new GameObject("VirtualJoystick");
        joystickUI.transform.SetParent(targetCanvas.transform, false);
        
        // Configurar RectTransform del contenedor
        RectTransform containerRect = joystickUI.AddComponent<RectTransform>();
        containerRect.anchorMin = new Vector2(0, 0);
        containerRect.anchorMax = new Vector2(0, 0);
        containerRect.anchoredPosition = joystickPosition;
        containerRect.sizeDelta = new Vector2(joystickSize, joystickSize);
        
        // Crear el fondo del joystick
        GameObject background = new GameObject("Background");
        background.transform.SetParent(joystickUI.transform, false);
        
        joystickBackground = background.AddComponent<RectTransform>();
        joystickBackground.anchorMin = Vector2.zero;
        joystickBackground.anchorMax = Vector2.one;
        joystickBackground.offsetMin = Vector2.zero;
        joystickBackground.offsetMax = Vector2.zero;
        
        Image backgroundImage = background.AddComponent<Image>();
        backgroundImage.color = backgroundColor;
        backgroundImage.sprite = CreateCircleSprite();
        backgroundImage.type = Image.Type.Simple;
        
        // Crear el handle del joystick
        GameObject handle = new GameObject("Handle");
        handle.transform.SetParent(background.transform, false);
        
        joystickHandle = handle.AddComponent<RectTransform>();
        joystickHandle.anchorMin = new Vector2(0.5f, 0.5f);
        joystickHandle.anchorMax = new Vector2(0.5f, 0.5f);
        joystickHandle.anchoredPosition = Vector2.zero;
        joystickHandle.sizeDelta = new Vector2(joystickSize * 0.4f, joystickSize * 0.4f);
        
        Image handleImage = handle.AddComponent<Image>();
        handleImage.color = handleColor;
        handleImage.sprite = CreateCircleSprite();
        handleImage.type = Image.Type.Simple;
        
        // Agregar el script VirtualJoystick
        virtualJoystick = joystickUI.AddComponent<VirtualJoystick>();
        virtualJoystick.joystickBackground = joystickBackground;
        virtualJoystick.joystickHandle = joystickHandle;
        virtualJoystick.joystickRange = joystickSize * 0.3f; // 30% del tamaño total
        
        Debug.Log("JoystickUISetup: UI del joystick creada correctamente");
    }
    
    private void ConfigureJoystick()
    {
        if (virtualJoystick == null) return;
        
        // Configurar parámetros del joystick
        virtualJoystick.mostrarJoystick = true;
        virtualJoystick.sensibilidad = 1f;
        virtualJoystick.deadZone = 0.1f;
        virtualJoystick.animarHandle = true;
        virtualJoystick.animacionVelocidad = 15f;
        
        Debug.Log("JoystickUISetup: Joystick configurado correctamente");
    }
    
    private Sprite CreateCircleSprite()
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
    /// Configura la posición del joystick en la pantalla
    /// </summary>
    public void SetJoystickPosition(Vector2 position)
    {
        joystickPosition = position;
        if (joystickUI != null)
        {
            RectTransform rect = joystickUI.GetComponent<RectTransform>();
            rect.anchoredPosition = position;
        }
    }
    
    /// <summary>
    /// Configura el tamaño del joystick
    /// </summary>
    public void SetJoystickSize(float size)
    {
        joystickSize = size;
        if (joystickUI != null)
        {
            RectTransform rect = joystickUI.GetComponent<RectTransform>();
            rect.sizeDelta = new Vector2(size, size);
            
            if (virtualJoystick != null)
            {
                virtualJoystick.joystickRange = size * 0.3f;
            }
        }
    }
    
    /// <summary>
    /// Obtiene la referencia al joystick virtual
    /// </summary>
    public VirtualJoystick GetVirtualJoystick()
    {
        return virtualJoystick;
    }
    
    /// <summary>
    /// Muestra u oculta el joystick
    /// </summary>
    public void SetJoystickVisible(bool visible)
    {
        if (joystickUI != null)
        {
            joystickUI.SetActive(visible);
        }
    }
}
