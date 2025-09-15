using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Target Settings")]
    [SerializeField] private Transform target; // El jugador a seguir
    [SerializeField] private Vector3 offset = new Vector3(0, 5, -10); // Distancia de la cámara al jugador
    
    [Header("Mouse Look Settings")]
    [SerializeField] private float mouseSensitivity = 2f; // Sensibilidad del mouse
    [SerializeField] private float maxLookAngle = 80f; // Ángulo máximo de mirada vertical
    [SerializeField] private bool invertY = false; // Invertir eje Y del mouse
    
    [Header("Follow Settings")]
    [SerializeField] private float followSpeed = 3f; // Velocidad de seguimiento
    
    // Variables privadas
    private float VelX = 0f;
    private float VelY = 0f;
    
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
                Debug.LogWarning("No se encontró un jugador con tag 'Player'. Asigna manualmente el target en el inspector.");
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
            VelX = angles.y;
            VelY = angles.x;
        }
    }
    
    void Update()
    {
        HandleInput();
    }
    
    void LateUpdate()
    {
        if (target == null) 
        {
            Debug.LogWarning("No hay target asignado para la cámara");
            return;
        }
        
        UpdateCameraPosition();
    }
    
    void HandleInput()
    {
        // Mouse look (siempre activo)
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;
        
        if (invertY)
            mouseY = -mouseY;
        
        VelX += mouseX;
        VelY -= mouseY;
        
        // Limitar ángulo vertical
        VelY = Mathf.Clamp(VelY, -maxLookAngle, maxLookAngle);
    }
    
    void UpdateCameraPosition()
    {
        // Calcular rotación
        Quaternion rotation = Quaternion.Euler(VelY, VelX, 0);
        
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
        Debug.Log("Target cambiado a: " + (newTarget != null ? newTarget.name : "null"));
    }
    
    public void SetMouseSensitivity(float newSensitivity)
    {
        mouseSensitivity = newSensitivity;
    }
}
