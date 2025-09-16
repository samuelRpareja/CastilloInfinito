using UnityEngine;
using UnityEngine.UI;

public class VirtualJoystick : MonoBehaviour
{
    [Header("Joystick Virtual")]
    public RectTransform joystickBackground;
    public RectTransform joystickHandle;
    public float joystickRange = 50f;
    
    [Header("Botón de Salto")]
    public RectTransform botonSalto;
    public bool mostrarBotonSalto = true;
    
    [Header("Configuración")]
    public bool mostrarJoystick = true;
    public float sensibilidad = 1f;
    
    private Vector2 joystickInput;
    private Vector2 joystickCenter;
    private bool isDragging = false;
    private bool botonSaltoPresionado = false;
    
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
        
        // Configurar botón de salto
        if (botonSalto != null)
        {
            botonSalto.gameObject.SetActive(mostrarBotonSalto);
        }
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
            for (int i = 0; i < Input.touchCount; i++)
            {
                Touch touch = Input.GetTouch(i);
                Vector2 touchPosition = touch.position;
                
                // Verificar si toca el botón de salto
                if (botonSalto != null && RectTransformUtility.RectangleContainsScreenPoint(botonSalto, touchPosition))
                {
                    switch (touch.phase)
                    {
                        case TouchPhase.Began:
                            botonSaltoPresionado = true;
                            break;
                        case TouchPhase.Ended:
                        case TouchPhase.Canceled:
                            botonSaltoPresionado = false;
                            break;
                    }
                }
                // Verificar si toca el joystick
                else if (RectTransformUtility.RectangleContainsScreenPoint(joystickBackground, touchPosition))
                {
                    switch (touch.phase)
                    {
                        case TouchPhase.Began:
                            isDragging = true;
                            ActualizarJoystick(touchPosition);
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
            }
        }
        else
        {
            isDragging = false;
            botonSaltoPresionado = false;
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
    
    public bool GetJumpButton()
    {
        return botonSaltoPresionado;
    }
}
