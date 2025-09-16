using UnityEngine;
using UnityEngine.UI;

public class VirtualJoystick : MonoBehaviour
{
    [Header("Joystick Virtual")]
    public RectTransform joystickBackground;
    public RectTransform joystickHandle;
    public float joystickRange = 50f;
    
    [Header("Configuración")]
    public bool mostrarJoystick = true;
    public float sensibilidad = 1f;
    
    private Vector2 joystickInput;
    private Vector2 joystickCenter;
    private bool isDragging = false;
    
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
            Touch touch = Input.GetTouch(0);
            Vector2 touchPosition = touch.position;
            
            switch (touch.phase)
            {
                case TouchPhase.Began:
                    if (RectTransformUtility.RectangleContainsScreenPoint(joystickBackground, touchPosition))
                    {
                        isDragging = true;
                        ActualizarJoystick(touchPosition);
                    }
                    break;
                    
                case TouchPhase.Moved:
                    if (isDragging)
                    {
                        ActualizarJoystick(touchPosition);
                    }
                    break;
                    
                case TouchPhase.Ended:
                case TouchPhase.Canceled:
                    isDragging = false;
                    ResetearJoystick();
                    break;
            }
        }
        else
        {
            isDragging = false;
            ResetearJoystick();
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
        
        joystickHandle.anchoredPosition = direction;
        
        // Calcular input normalizado
        joystickInput = direction / joystickRange * sensibilidad;
    }
    
    void ResetearJoystick()
    {
        joystickHandle.anchoredPosition = Vector2.zero;
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
}
