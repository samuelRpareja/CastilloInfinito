using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Creador automático de Canvas para la UI del joystick
/// Se puede usar como alternativa si no tienes Canvas en la escena
/// </summary>
public class CanvasSetup : MonoBehaviour
{
    [Header("Configuración del Canvas")]
    [SerializeField] private bool crearCanvasAutomaticamente = true;
    [SerializeField] private CanvasScaler.ScaleMode scaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
    [SerializeField] private Vector2 referenceResolution = new Vector2(1920, 1080);
    [SerializeField] private CanvasScaler.ScreenMatchMode matchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
    [SerializeField] private float matchWidthOrHeight = 0.5f;
    
    [Header("Configuración de Render")]
    [SerializeField] private RenderMode renderMode = RenderMode.ScreenSpaceOverlay;
    [SerializeField] private int sortOrder = 0;
    
    private Canvas canvas;
    private CanvasScaler canvasScaler;
    private GraphicRaycaster graphicRaycaster;
    
    private void Awake()
    {
        if (crearCanvasAutomaticamente)
        {
            CrearCanvas();
        }
    }
    
    /// <summary>
    /// Crea un Canvas completo con todas las configuraciones necesarias
    /// </summary>
    [ContextMenu("Crear Canvas")]
    public void CrearCanvas()
    {
        // Verificar si ya existe un Canvas
        Canvas existingCanvas = FindObjectOfType<Canvas>();
        if (existingCanvas != null)
        {
            Debug.Log("CanvasSetup: Ya existe un Canvas en la escena");
            canvas = existingCanvas;
            return;
        }
        
        // Crear el GameObject del Canvas
        GameObject canvasObject = new GameObject("Canvas");
        canvasObject.transform.SetParent(transform, false);
        
        // Agregar componente Canvas
        canvas = canvasObject.AddComponent<Canvas>();
        canvas.renderMode = renderMode;
        canvas.sortingOrder = sortOrder;
        
        // Agregar CanvasScaler para escalado automático
        canvasScaler = canvasObject.AddComponent<CanvasScaler>();
        canvasScaler.uiScaleMode = scaleMode;
        canvasScaler.referenceResolution = referenceResolution;
        canvasScaler.screenMatchMode = matchMode;
        canvasScaler.matchWidthOrHeight = matchWidthOrHeight;
        
        // Agregar GraphicRaycaster para interacción con UI
        graphicRaycaster = canvasObject.AddComponent<GraphicRaycaster>();
        
        // Configurar para Android
        #if UNITY_ANDROID
        ConfigurarParaAndroid();
        #endif
        
        Debug.Log("CanvasSetup: Canvas creado correctamente");
    }
    
    private void ConfigurarParaAndroid()
    {
        if (canvasScaler == null) return;
        
        // Configuraciones optimizadas para Android
        canvasScaler.referenceResolution = new Vector2(1080, 1920); // Resolución típica de móvil
        canvasScaler.matchWidthOrHeight = 0.5f; // Balance entre ancho y alto
        
        Debug.Log("CanvasSetup: Canvas configurado para Android");
    }
    
    /// <summary>
    /// Obtiene el Canvas creado
    /// </summary>
    public Canvas GetCanvas()
    {
        return canvas;
    }
    
    /// <summary>
    /// Configura el Canvas para una resolución específica
    /// </summary>
    public void SetReferenceResolution(Vector2 resolution)
    {
        referenceResolution = resolution;
        if (canvasScaler != null)
        {
            canvasScaler.referenceResolution = resolution;
        }
    }
    
    /// <summary>
    /// Configura el modo de escalado del Canvas
    /// </summary>
    public void SetScaleMode(CanvasScaler.ScaleMode mode)
    {
        scaleMode = mode;
        if (canvasScaler != null)
        {
            canvasScaler.uiScaleMode = mode;
        }
    }
    
    /// <summary>
    /// Configura el orden de renderizado del Canvas
    /// </summary>
    public void SetSortOrder(int order)
    {
        sortOrder = order;
        if (canvas != null)
        {
            canvas.sortingOrder = order;
        }
    }
}
