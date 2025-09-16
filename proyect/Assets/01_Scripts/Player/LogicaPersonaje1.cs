using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LogicaPersonaje1 : MonoBehaviour
{
    [Header("Configuración de Movimiento")]
    public float velocidadMovimiento = 5.0f;
    public float velocidadRotacion = 200.0f;
    
    [Header("Joystick")]
    public VirtualJoystick joystick;
    
    private Animator anim;
    private float x, y;

    void Start()
    {
        anim = GetComponent<Animator>();
        
        // Buscar joystick si no está asignado
        if (joystick == null)
        {
            joystick = FindObjectOfType<VirtualJoystick>();
        }
        
        // Configurar para Android
        #if UNITY_ANDROID
        Application.targetFrameRate = 60;
        QualitySettings.vSyncCount = 1;
        #endif
    }

    void Update()
    {
        // Obtener input según la plataforma
        if (Application.isMobilePlatform && joystick != null)
        {
            // Usar joystick virtual en móvil
            x = joystick.GetHorizontal();
            y = joystick.GetVertical();
        }
        else
        {
            // Usar teclado en PC
            x = Input.GetAxis("Horizontal");
            y = Input.GetAxis("Vertical");
        }

        // Aplicar movimiento
        transform.Rotate(0, x * Time.deltaTime * velocidadRotacion, 0);
        transform.Translate(0, 0, y * Time.deltaTime * velocidadMovimiento);

        // Actualizar animaciones
        if (anim != null)
        {
            anim.SetFloat("VelX", x);
            anim.SetFloat("VelY", y);
        }
    }
}
