using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;

public class NetcodeSetup : MonoBehaviour
{
    [Header("Configuración de Prefabs")]
    public GameObject playerPrefab;
    public GameObject enemyPrefab;
    
    [Header("Configuración de Red")]
    public string serverIP = "127.0.0.1";
    public ushort serverPort = 7777;
    
    [Header("UI Elements")]
    public Canvas multiplayerCanvas;
    public Button hostButton;
    public Button clientButton;
    public Button serverButton;
    public Button disconnectButton;
    public InputField playerNameInput;
    public InputField serverAddressInput;
    public Text connectionStatusText;
    public Text playerCountText;
    
    [ContextMenu("Configurar Netcode Automáticamente")]
    public void SetupNetcode()
    {
        Debug.Log("🔧 Configurando Netcode for GameObjects...");
        
        // 1. Configurar NetworkManager
        SetupNetworkManager();
        
        // 2. Configurar Prefabs
        SetupPrefabs();
        
        // 3. Configurar UI
        SetupUI();
        
        Debug.Log("✅ Configuración de Netcode completada!");
    }
    
    private void SetupNetworkManager()
    {
        // Buscar NetworkManager en la escena
        var networkManager = FindObjectOfType<Unity.Netcode.NetworkManager>();
        if (networkManager == null)
        {
            Debug.LogError("❌ No se encontró NetworkManager en la escena. Agrega el prefab NetworkManager de Netcode.");
            return;
        }
        
        // Configurar NetworkManager
        if (playerPrefab != null)
        {
            networkManager.NetworkConfig.PlayerPrefab = playerPrefab;
        }
        
        // Configurar transporte
        var transport = networkManager.GetComponent<Unity.Netcode.Transports.UTP.UnityTransport>();
        if (transport != null)
        {
            transport.ConnectionData.Address = serverIP;
            transport.ConnectionData.Port = serverPort;
        }
        
        Debug.Log("✅ NetworkManager configurado");
    }
    
    private void SetupPrefabs()
    {
        if (playerPrefab != null)
        {
            // Agregar componentes de red al prefab del jugador
            AddNetworkComponents(playerPrefab, true);
            Debug.Log("✅ Prefab del jugador configurado");
        }
        
        if (enemyPrefab != null)
        {
            // Agregar componentes de red al prefab del enemigo
            AddNetworkComponents(enemyPrefab, false);
            Debug.Log("✅ Prefab del enemigo configurado");
        }
    }
    
    private void AddNetworkComponents(GameObject prefab, bool isPlayer)
    {
        // Agregar NetworkObject
        if (prefab.GetComponent<NetworkObject>() == null)
        {
            prefab.AddComponent<NetworkObject>();
        }
        
        // Agregar script de red específico
        if (isPlayer)
        {
            if (prefab.GetComponent<NetworkPlayer>() == null)
            {
                prefab.AddComponent<NetworkPlayer>();
            }
            
            // Agregar controlador de input para jugadores
            if (prefab.GetComponent<PlayerInputController>() == null)
            {
                prefab.AddComponent<PlayerInputController>();
            }
        }
        else
        {
            if (prefab.GetComponent<NetworkEnemy>() == null)
            {
                prefab.AddComponent<NetworkEnemy>();
            }
        }
    }
    
    private void SetupUI()
    {
        if (multiplayerCanvas == null)
        {
            // Crear Canvas si no existe
            GameObject canvasGO = new GameObject("MultiplayerCanvas");
            multiplayerCanvas = canvasGO.AddComponent<Canvas>();
            multiplayerCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasGO.AddComponent<CanvasScaler>();
            canvasGO.AddComponent<GraphicRaycaster>();
        }
        
        // Crear UI básica si no existe
        CreateBasicUI();
        
        // Configurar NetworkUI
        NetworkUI networkUI = FindObjectOfType<NetworkUI>();
        if (networkUI == null)
        {
            GameObject uiGO = new GameObject("NetworkUI");
            networkUI = uiGO.AddComponent<NetworkUI>();
        }
        
        // Asignar referencias
        networkUI.mainMenuPanel = GameObject.Find("MainMenuPanel");
        networkUI.gamePanel = GameObject.Find("GamePanel");
        networkUI.connectionPanel = GameObject.Find("ConnectionPanel");
        networkUI.hostButton = hostButton;
        networkUI.clientButton = clientButton;
        networkUI.disconnectButton = disconnectButton;
        networkUI.playerNameInput = playerNameInput;
        networkUI.serverAddressInput = serverAddressInput;
        networkUI.connectionStatusText = connectionStatusText;
        networkUI.playerCountText = playerCountText;
        
        Debug.Log("✅ UI configurada");
    }
    
    private void CreateBasicUI()
    {
        // Crear panel principal
        GameObject mainPanel = new GameObject("MainMenuPanel");
        mainPanel.transform.SetParent(multiplayerCanvas.transform, false);
        
        // Crear botones básicos
        if (hostButton == null)
        {
            hostButton = CreateButton("HostButton", "Iniciar Host", mainPanel.transform);
        }
        
        if (clientButton == null)
        {
            clientButton = CreateButton("ClientButton", "Conectar", mainPanel.transform);
        }
        
        if (serverButton == null)
        {
            serverButton = CreateButton("ServerButton", "Iniciar Servidor", mainPanel.transform);
        }
        
        if (disconnectButton == null)
        {
            disconnectButton = CreateButton("DisconnectButton", "Desconectar", mainPanel.transform);
        }
        
        // Crear campos de entrada
        if (playerNameInput == null)
        {
            playerNameInput = CreateInputField("PlayerNameInput", "Nombre del Jugador", mainPanel.transform);
        }
        
        if (serverAddressInput == null)
        {
            serverAddressInput = CreateInputField("ServerAddressInput", "localhost", mainPanel.transform);
        }
        
        // Crear textos
        if (connectionStatusText == null)
        {
            connectionStatusText = CreateText("ConnectionStatusText", "Desconectado", mainPanel.transform);
        }
        
        if (playerCountText == null)
        {
            playerCountText = CreateText("PlayerCountText", "Jugadores: 0", mainPanel.transform);
        }
    }
    
    private Button CreateButton(string name, string text, Transform parent)
    {
        GameObject buttonGO = new GameObject(name);
        buttonGO.transform.SetParent(parent, false);
        
        Image image = buttonGO.AddComponent<Image>();
        Button button = buttonGO.AddComponent<Button>();
        
        // Crear texto del botón
        GameObject textGO = new GameObject("Text");
        textGO.transform.SetParent(buttonGO.transform, false);
        
        Text buttonText = textGO.AddComponent<Text>();
        buttonText.text = text;
        buttonText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        buttonText.color = Color.black;
        buttonText.alignment = TextAnchor.MiddleCenter;
        
        // Configurar RectTransform
        RectTransform rectTransform = textGO.GetComponent<RectTransform>();
        rectTransform.anchorMin = Vector2.zero;
        rectTransform.anchorMax = Vector2.one;
        rectTransform.offsetMin = Vector2.zero;
        rectTransform.offsetMax = Vector2.zero;
        
        return button;
    }
    
    private InputField CreateInputField(string name, string placeholder, Transform parent)
    {
        GameObject inputGO = new GameObject(name);
        inputGO.transform.SetParent(parent, false);
        
        Image image = inputGO.AddComponent<Image>();
        InputField inputField = inputGO.AddComponent<InputField>();
        
        // Crear texto del placeholder
        GameObject placeholderGO = new GameObject("Placeholder");
        placeholderGO.transform.SetParent(inputGO.transform, false);
        
        Text placeholderText = placeholderGO.AddComponent<Text>();
        placeholderText.text = placeholder;
        placeholderText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        placeholderText.color = Color.gray;
        placeholderText.alignment = TextAnchor.MiddleLeft;
        
        // Crear texto de entrada
        GameObject textGO = new GameObject("Text");
        textGO.transform.SetParent(inputGO.transform, false);
        
        Text inputText = textGO.AddComponent<Text>();
        inputText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        inputText.color = Color.black;
        inputText.alignment = TextAnchor.MiddleLeft;
        
        // Configurar InputField
        inputField.textComponent = inputText;
        inputField.placeholder = placeholderText;
        
        // Configurar RectTransforms
        RectTransform inputRect = inputGO.GetComponent<RectTransform>();
        inputRect.sizeDelta = new Vector2(200, 30);
        
        RectTransform placeholderRect = placeholderGO.GetComponent<RectTransform>();
        placeholderRect.anchorMin = Vector2.zero;
        placeholderRect.anchorMax = Vector2.one;
        placeholderRect.offsetMin = new Vector2(10, 0);
        placeholderRect.offsetMax = new Vector2(-10, 0);
        
        RectTransform textRect = textGO.GetComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = new Vector2(10, 0);
        textRect.offsetMax = new Vector2(-10, 0);
        
        return inputField;
    }
    
    private Text CreateText(string name, string text, Transform parent)
    {
        GameObject textGO = new GameObject(name);
        textGO.transform.SetParent(parent, false);
        
        Text textComponent = textGO.AddComponent<Text>();
        textComponent.text = text;
        textComponent.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        textComponent.color = Color.white;
        textComponent.alignment = TextAnchor.MiddleCenter;
        
        return textComponent;
    }
    
    [ContextMenu("Probar Conexión Local")]
    public void TestLocalConnection()
    {
        if (Unity.Netcode.NetworkManager.Singleton != null)
        {
            Debug.Log("🧪 Iniciando prueba de conexión local...");
            Unity.Netcode.NetworkManager.Singleton.StartHost();
        }
        else
        {
            Debug.LogError("❌ No se encontró NetworkManager");
        }
    }
}
