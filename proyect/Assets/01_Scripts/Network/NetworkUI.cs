using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;

public class NetworkUI : MonoBehaviour
{
    [Header("UI Elements")]
    public GameObject mainMenuPanel;
    public GameObject gamePanel;
    public GameObject connectionPanel;
    
    [Header("Buttons")]
    public Button hostButton;
    public Button clientButton;
    public Button disconnectButton;
    
    [Header("Input Fields")]
    public InputField playerNameInput;
    public InputField serverAddressInput;
    
    [Header("Text Elements")]
    public Text connectionStatusText;
    public Text playerCountText;
    
    private NetworkManager networkManager;
    
    private void Start()
    {
        networkManager = FindObjectOfType<NetworkManager>();
        if (networkManager == null)
        {
            Debug.LogError("❌ No se encontró NetworkManager en la escena!");
            return;
        }
        
        SetupUI();
        SetupEventListeners();
    }
    
    private void SetupUI()
    {
        // Configurar valores por defecto
        if (playerNameInput != null)
            playerNameInput.text = "Jugador" + Random.Range(1, 1000);
        
        if (serverAddressInput != null)
            serverAddressInput.text = "localhost";
        
        // Mostrar panel principal por defecto
        ShowMainMenu();
    }
    
    private void SetupEventListeners()
    {
        if (hostButton != null)
            hostButton.onClick.AddListener(StartHost);
        
        if (clientButton != null)
            clientButton.onClick.AddListener(StartClient);
        
        if (disconnectButton != null)
            disconnectButton.onClick.AddListener(Disconnect);
    }
    
    private void Update()
    {
        UpdateConnectionStatus();
        UpdatePlayerCount();
    }
    
    private void UpdateConnectionStatus()
    {
        if (connectionStatusText == null) return;
        
        if (Unity.Netcode.NetworkManager.Singleton != null)
        {
            if (Unity.Netcode.NetworkManager.Singleton.IsClient)
            {
                connectionStatusText.text = "🟢 Conectado";
                connectionStatusText.color = Color.green;
            }
            else if (Unity.Netcode.NetworkManager.Singleton.IsServer)
            {
                connectionStatusText.text = "🟡 Servidor Activo";
                connectionStatusText.color = Color.yellow;
            }
            else
            {
                connectionStatusText.text = "🔴 Desconectado";
                connectionStatusText.color = Color.red;
            }
        }
        else
        {
            connectionStatusText.text = "🔴 Desconectado";
            connectionStatusText.color = Color.red;
        }
    }
    
    private void UpdatePlayerCount()
    {
        if (playerCountText == null) return;
        
        int playerCount = 0;
        if (Unity.Netcode.NetworkManager.Singleton != null)
        {
            if (Unity.Netcode.NetworkManager.Singleton.IsServer)
            {
                playerCount = Unity.Netcode.NetworkManager.Singleton.ConnectedClients.Count;
            }
            else if (Unity.Netcode.NetworkManager.Singleton.IsClient)
            {
                playerCount = 1;
            }
        }
        
        playerCountText.text = $"Jugadores: {playerCount}";
    }
    
    public void StartHost()
    {
        Debug.Log("🏠 Iniciando Host...");
        
        if (Unity.Netcode.NetworkManager.Singleton != null)
        {
            Unity.Netcode.NetworkManager.Singleton.StartHost();
            ShowGamePanel();
        }
    }
    
    public void StartClient()
    {
        Debug.Log("🔗 Conectando como Cliente...");
        
        if (Unity.Netcode.NetworkManager.Singleton != null)
        {
            Unity.Netcode.NetworkManager.Singleton.StartClient();
            ShowConnectionPanel();
        }
    }
    
    public void StartServer()
    {
        Debug.Log("🖥️ Iniciando Servidor...");
        
        if (Unity.Netcode.NetworkManager.Singleton != null)
        {
            Unity.Netcode.NetworkManager.Singleton.StartServer();
            ShowGamePanel();
        }
    }
    
    public void Disconnect()
    {
        Debug.Log("🔌 Desconectando...");
        
        if (Unity.Netcode.NetworkManager.Singleton != null)
        {
            Unity.Netcode.NetworkManager.Singleton.Shutdown();
        }
        
        ShowMainMenu();
    }
    
    private void ShowMainMenu()
    {
        if (mainMenuPanel != null)
            mainMenuPanel.SetActive(true);
        if (gamePanel != null)
            gamePanel.SetActive(false);
        if (connectionPanel != null)
            connectionPanel.SetActive(false);
    }
    
    private void ShowGamePanel()
    {
        if (mainMenuPanel != null)
            mainMenuPanel.SetActive(false);
        if (gamePanel != null)
            gamePanel.SetActive(true);
        if (connectionPanel != null)
            connectionPanel.SetActive(false);
    }
    
    private void ShowConnectionPanel()
    {
        if (mainMenuPanel != null)
            mainMenuPanel.SetActive(false);
        if (gamePanel != null)
            gamePanel.SetActive(false);
        if (connectionPanel != null)
            connectionPanel.SetActive(true);
    }
    
    // Métodos para ser llamados desde otros scripts
    public void OnPlayerConnected()
    {
        ShowGamePanel();
    }
    
    public void OnPlayerDisconnected()
    {
        ShowMainMenu();
    }
    
    private void OnDestroy()
    {
        // Limpiar event listeners
        if (hostButton != null)
            hostButton.onClick.RemoveListener(StartHost);
        if (clientButton != null)
            clientButton.onClick.RemoveListener(StartClient);
        if (disconnectButton != null)
            disconnectButton.onClick.RemoveListener(Disconnect);
    }
}
