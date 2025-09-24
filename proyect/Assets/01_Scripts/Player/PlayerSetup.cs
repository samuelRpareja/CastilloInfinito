using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Configurador del PlayerController con prefab del jugador
/// Ayuda a conectar el sistema de joystick con tu prefab existente
/// </summary>
public class PlayerSetup : MonoBehaviour
{
    [Header("Configuración del Jugador")]
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private bool instanciarJugadorAutomaticamente = true;
    
    [Header("Configuración del Sistema")]
    [SerializeField] private bool configurarJoystickAutomaticamente = true;
    [SerializeField] private bool usarCanvasExistente = true;
    
    [Header("Referencias")]
    [SerializeField] private Canvas targetCanvas;
    [SerializeField] private PlayerController playerController;
    
    private GameObject playerInstance;
    
    private void Start()
    {
        if (instanciarJugadorAutomaticamente)
        {
            ConfigurarJugadorCompleto();
        }
    }
    
    /// <summary>
    /// Configura todo el sistema del jugador automáticamente
    /// </summary>
    [ContextMenu("Configurar Jugador Completo")]
    public void ConfigurarJugadorCompleto()
    {
        Debug.Log("PlayerSetup: Iniciando configuración completa del jugador...");
        
        // 1. Crear/Configurar Canvas
        ConfigurarCanvas();
        
        // 2. Instanciar el jugador
        InstanciarJugador();
        
        // 3. Configurar el sistema de joystick
        if (configurarJoystickAutomaticamente)
        {
            ConfigurarSistemaJoystick();
        }
        
        Debug.Log("PlayerSetup: Configuración del jugador completada");
    }
    
    private void ConfigurarCanvas()
    {
        // Buscar Canvas existente
        if (usarCanvasExistente)
        {
            targetCanvas = FindObjectOfType<Canvas>();
        }
        
        // Si no hay Canvas, crear uno
        if (targetCanvas == null)
        {
            GameObject canvasObject = new GameObject("Canvas");
            targetCanvas = canvasObject.AddComponent<Canvas>();
            targetCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
            
            // Agregar CanvasScaler
            CanvasScaler scaler = canvasObject.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            
            // Agregar GraphicRaycaster
            canvasObject.AddComponent<GraphicRaycaster>();
            
            Debug.Log("PlayerSetup: Canvas creado automáticamente");
        }
        else
        {
            Debug.Log("PlayerSetup: Usando Canvas existente");
        }
    }
    
    private void InstanciarJugador()
    {
        if (playerPrefab == null)
        {
            Debug.LogError("PlayerSetup: No se ha asignado el prefab del jugador");
            return;
        }
        
        // Determinar posición de spawn
        Vector3 spawnPosition = Vector3.zero;
        if (spawnPoint != null)
        {
            spawnPosition = spawnPoint.position;
        }
        
        // Instanciar el jugador
        playerInstance = Instantiate(playerPrefab, spawnPosition, Quaternion.identity);
        playerInstance.name = "Player";
        
        // Obtener el PlayerController
        playerController = playerInstance.GetComponent<PlayerController>();
        if (playerController == null)
        {
            Debug.LogWarning("PlayerSetup: El prefab del jugador no tiene PlayerController");
        }
        
        Debug.Log("PlayerSetup: Jugador instanciado correctamente");
    }
    
    private void ConfigurarSistemaJoystick()
    {
        if (playerController == null)
        {
            Debug.LogError("PlayerSetup: No se puede configurar el joystick sin PlayerController");
            return;
        }
        
        // Agregar el configurador automático del joystick
        AndroidJoystickSetup joystickSetup = FindObjectOfType<AndroidJoystickSetup>();
        if (joystickSetup == null)
        {
            GameObject setupObject = new GameObject("JoystickSetup");
            joystickSetup = setupObject.AddComponent<AndroidJoystickSetup>();
        }
        
        // Configurar el Canvas objetivo
        if (targetCanvas != null)
        {
            // Usar reflexión para asignar el Canvas (ya que es privado)
            var field = typeof(AndroidJoystickSetup).GetField("targetCanvas", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (field != null)
            {
                field.SetValue(joystickSetup, targetCanvas);
            }
        }
        
        // Configurar el PlayerController
        var playerControllerField = typeof(AndroidJoystickSetup).GetField("playerController", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        if (playerControllerField != null)
        {
            playerControllerField.SetValue(joystickSetup, playerController);
        }
        
        // Ejecutar la configuración
        joystickSetup.ConfigurarSistemaCompleto();
        
        Debug.Log("PlayerSetup: Sistema de joystick configurado");
    }
    
    /// <summary>
    /// Asigna el prefab del jugador
    /// </summary>
    public void SetPlayerPrefab(GameObject prefab)
    {
        playerPrefab = prefab;
    }
    
    /// <summary>
    /// Asigna el punto de spawn del jugador
    /// </summary>
    public void SetSpawnPoint(Transform spawn)
    {
        spawnPoint = spawn;
    }
    
    /// <summary>
    /// Asigna el Canvas objetivo
    /// </summary>
    public void SetTargetCanvas(Canvas canvas)
    {
        targetCanvas = canvas;
    }
    
    /// <summary>
    /// Obtiene la instancia del jugador
    /// </summary>
    public GameObject GetPlayerInstance()
    {
        return playerInstance;
    }
    
    /// <summary>
    /// Obtiene el PlayerController
    /// </summary>
    public PlayerController GetPlayerController()
    {
        return playerController;
    }
    
    /// <summary>
    /// Reinstancia el jugador en una nueva posición
    /// </summary>
    public void RespawnPlayer(Vector3 newPosition)
    {
        if (playerInstance != null)
        {
            DestroyImmediate(playerInstance);
        }
        
        playerInstance = Instantiate(playerPrefab, newPosition, Quaternion.identity);
        playerInstance.name = "Player";
        playerController = playerInstance.GetComponent<PlayerController>();
        
        Debug.Log("PlayerSetup: Jugador respawneado");
    }
}
