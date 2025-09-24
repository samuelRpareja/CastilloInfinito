using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Joystick virtual para dispositivos táctiles
/// Optimizado para Android con detección de toques mejorada
/// </summary>
public class VirtualJoystick : MonoBehaviour
{
    [Header("Joystick Virtual")]
    public RectTransform joystickBackground;
    public RectTransform joystickHandle;
    public float joystickRange = 50f;
    
    [Header("Configuración")]
    public bool mostrarJoystick = true;
    public float sensibilidad = 1f;
    public float deadZone = 0.1f;
    
    [Header("Visual")]
    public bool animarHandle = true;
    public float animacionVelocidad = 10f;
    
    private Vector2 joystickInput;
    private Vector2 joystickCenter;
    private bool isDragging = false;
    private int touchId = -1; // ID del touch que controla el joystick
    
    
    void Start()
    {
        // Configurar para Android
        #if UNITY_ANDROID
        mostrarJoystick = true;
        #else
        mostrarJoystick = false;
        #endif
        
        if (joystickBackground != null)
        {
            joystickCenter = joystickBackground.anchoredPosition;
        }
        
        // Mostrar/ocultar joystick según la plataforma
        gameObject.SetActive(mostrarJoystick);
        
        // Sin botón de salto
    }
    
    void Update()
    {
        if (!mostrarJoystick) return;
        
        ManejarInput();
    }
    
    void ManejarInput()
    {
        if (Input.touchCount > 0)
        {
            // Si ya tenemos un touch asignado, solo procesar ese
            if (touchId >= 0)
            {
                for (int i = 0; i < Input.touchCount; i++)
                {
                    Touch touch = Input.GetTouch(i);
                    if (touch.fingerId == touchId)
                    {
                        ProcesarTouch(touch);
                        return;
                    }
                }
                // Si no encontramos nuestro touch, resetear
                ResetearJoystick();
                touchId = -1;
            }
            
            // Buscar un nuevo touch en el área del joystick
            for (int i = 0; i < Input.touchCount; i++)
            {
                Touch touch = Input.GetTouch(i);
                Vector2 touchPosition = touch.position;
                
                if (touch.phase == TouchPhase.Began && 
                    RectTransformUtility.RectangleContainsScreenPoint(joystickBackground, touchPosition))
                {
                    touchId = touch.fingerId;
                    ProcesarTouch(touch);
                    break;
                }
            }
        }
        else
        {
            ResetearJoystick();
            touchId = -1;
        }
    }
    
    void ProcesarTouch(Touch touch)
    {
        switch (touch.phase)
        {
            case TouchPhase.Began:
                isDragging = true;
                ActualizarJoystick(touch.position);
                break;
                
            case TouchPhase.Moved:
                if (isDragging)
                {
                    ActualizarJoystick(touch.position);
                }
                break;
                
            case TouchPhase.Ended:
            case TouchPhase.Canceled:
                isDragging = false;
                ResetearJoystick();
                touchId = -1;
                break;
        }
    }
    
    void ActualizarJoystick(Vector2 touchPosition)
    {
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            joystickBackground, touchPosition, null, out localPoint);
        
        Vector2 direction = localPoint - joystickCenter;
        float distance = direction.magnitude;
        
        if (distance > joystickRange)
        {
            direction = direction.normalized * joystickRange;
        }
        
        // Actualizar posición del handle con animación suave
        if (animarHandle)
        {
            joystickHandle.anchoredPosition = Vector2.Lerp(
                joystickHandle.anchoredPosition, 
                direction, 
                Time.deltaTime * animacionVelocidad);
        }
        else
        {
            joystickHandle.anchoredPosition = direction;
        }
        
        // Calcular input normalizado
        joystickInput = direction / joystickRange * sensibilidad;
        
        // Aplicar dead zone
        if (Mathf.Abs(joystickInput.x) < deadZone) joystickInput.x = 0f;
        if (Mathf.Abs(joystickInput.y) < deadZone) joystickInput.y = 0f;
    }
    
    void ResetearJoystick()
    {
        if (animarHandle)
        {
            joystickHandle.anchoredPosition = Vector2.Lerp(
                joystickHandle.anchoredPosition, 
                Vector2.zero, 
                Time.deltaTime * animacionVelocidad);
        }
        else
        {
            joystickHandle.anchoredPosition = Vector2.zero;
        }
        
        joystickInput = Vector2.zero;
    }
    
    // Métodos públicos para obtener el input
    public float GetHorizontal()
    {
        return joystickInput.x;
    }
    
    public float GetVertical()
    {
        return joystickInput.y;
    }
    
    public Vector2 GetInput()
    {
        return joystickInput;
    }
    
    public bool IsPressed()
    {
        return isDragging;
    }
    
    /// <summary>
    /// Obtiene la magnitud del input (0-1)
    /// </summary>
    public float GetMagnitude()
    {
        return joystickInput.magnitude;
    }
    
    /// <summary>
    /// Obtiene la dirección del input como Vector2 normalizado
    /// </summary>
    public Vector2 GetDirection()
    {
        return joystickInput.normalized;
    }
    
    /// <summary>
    /// Configura el rango del joystick
    /// </summary>
    public void SetJoystickRange(float range)
    {
        joystickRange = range;
    }
    
    /// <summary>
    /// Configura la sensibilidad del joystick
    /// </summary>
    public void SetSensitivity(float sensitivity)
    {
        sensibilidad = sensitivity;
    }
    
    /// <summary>
    /// Configura la zona muerta del joystick
    /// </summary>
    public void SetDeadZone(float deadZoneValue)
    {
        deadZone = deadZoneValue;
    }
    
    /// <summary>
    /// Habilita o deshabilita la animación del handle
    /// </summary>
    public void SetHandleAnimation(bool animate)
    {
        animarHandle = animate;
    }
}
