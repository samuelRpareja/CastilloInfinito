using UnityEngine;

/// <summary>
/// Simple Camera Controller - Fallback version without SOLID complexity
/// </summary>
public class SimpleCameraController : MonoBehaviour
{
    [Header("Target Settings")]
    [SerializeField] public Transform target; // El jugador a seguir
    [SerializeField] private Vector3 offset = new Vector3(0, 5, -10); // Distancia de la cámara al jugador
    
    [Header("Mouse Look Settings")]
    [SerializeField] private float mouseSensitivity = 2f; // Sensibilidad del mouse
    [SerializeField] private float maxLookAngle = 80f; // Ángulo máximo de mirada vertical
    [SerializeField] private bool invertY = false; // Invertir eje Y del mouse
    
    [Header("Follow Settings")]
    [SerializeField] private float followSpeed = 5f; // Velocidad de seguimiento
    
    // Variables privadas
    private float mouseX = 0f;
    private float mouseY = 0f;
    
    void Start()
    {
        // Buscar automáticamente al jugador si no está asignado
        if (target == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                target = player.transform;
                Debug.Log("Jugador encontrado automáticamente: " + player.name);
            }
            else
            {
                // Buscar por componente PlayerMovement
                PlayerMovement playerMovement = FindObjectOfType<PlayerMovement>();
                if (playerMovement != null)
                {
                    target = playerMovement.transform;
                    Debug.Log("PlayerMovement encontrado: " + playerMovement.name);
                }
                else
                {
                    Debug.LogWarning("No se encontró un jugador. Asigna manualmente el target en el inspector.");
                }
            }
        }
        else
        {
            Debug.Log("Target asignado: " + target.name);
        }
        
        // Inicializar rotación
        if (target != null)
        {
            Vector3 angles = transform.eulerAngles;
            mouseX = angles.y;
            mouseY = angles.x;
        }
    }
    
    void Update()
    {
        HandleMouseInput();
    }
    
    void LateUpdate()
    {
        if (target == null) return;
        
        UpdateCameraPosition();
    }
    
    void HandleMouseInput()
    {
        // Mouse look
        float mouseXInput = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseYInput = Input.GetAxis("Mouse Y") * mouseSensitivity;
        
        if (invertY)
            mouseYInput = -mouseYInput;
        
        mouseX += mouseXInput;
        mouseY -= mouseYInput;
        
        // Limitar ángulo vertical
        mouseY = Mathf.Clamp(mouseY, -maxLookAngle, maxLookAngle);
    }
    
    void UpdateCameraPosition()
    {
        // Calcular rotación
        Quaternion rotation = Quaternion.Euler(mouseY, mouseX, 0);
        
        // Calcular posición objetivo
        Vector3 targetPosition = target.position + rotation * offset;
        
        // Aplicar seguimiento suave
        transform.position = Vector3.Lerp(transform.position, targetPosition, followSpeed * Time.deltaTime);
        
        // Mirar al jugador
        transform.LookAt(target);
    }
    
    // Métodos públicos
    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }
    
    public void SetMouseSensitivity(float newSensitivity)
    {
        mouseSensitivity = newSensitivity;
    }
}
