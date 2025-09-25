using UnityEngine;
using Unity.Netcode;

public class NetworkManager : MonoBehaviour
{
    [Header("Configuración del Jugador")]
    public GameObject playerPrefab;
    
    [Header("Configuración de la Sala")]
    public int maxPlayers = 4;
    
    [Header("Configuración de Red")]
    public string serverIP = "127.0.0.1";
    public ushort serverPort = 7777;
    
    private void Start()
    {
        // Configurar el NetworkManager de Unity
        if (Unity.Netcode.NetworkManager.Singleton != null)
        {
            var netManager = Unity.Netcode.NetworkManager.Singleton;
            netManager.OnClientConnectedCallback += OnClientConnected;
            netManager.OnClientDisconnectCallback += OnClientDisconnected;
            netManager.OnServerStarted += OnServerStarted;
        }
    }
    
    private void OnServerStarted()
    {
        Debug.Log("🟢 Servidor iniciado");
    }
    
    private void OnClientConnected(ulong clientId)
    {
        Debug.Log($"🔗 Cliente conectado: {clientId}");
    }
    
    private void OnClientDisconnected(ulong clientId)
    {
        Debug.Log($"🔌 Cliente desconectado: {clientId}");
    }
    
    // Métodos para UI
    public void StartHost()
    {
        Debug.Log("🏠 Iniciando Host...");
        if (Unity.Netcode.NetworkManager.Singleton != null)
        {
            Unity.Netcode.NetworkManager.Singleton.StartHost();
        }
    }
    
    public void StartClient()
    {
        Debug.Log("🔗 Conectando como Cliente...");
        if (Unity.Netcode.NetworkManager.Singleton != null)
        {
            Unity.Netcode.NetworkManager.Singleton.StartClient();
        }
    }
    
    public void StartServer()
    {
        Debug.Log("🖥️ Iniciando Servidor...");
        if (Unity.Netcode.NetworkManager.Singleton != null)
        {
            Unity.Netcode.NetworkManager.Singleton.StartServer();
        }
    }
    
    public void StopHost()
    {
        Debug.Log("🛑 Deteniendo Host...");
        if (Unity.Netcode.NetworkManager.Singleton != null)
        {
            Unity.Netcode.NetworkManager.Singleton.Shutdown();
        }
    }
    
    public void Disconnect()
    {
        Debug.Log("🔌 Desconectando...");
        if (Unity.Netcode.NetworkManager.Singleton != null)
        {
            Unity.Netcode.NetworkManager.Singleton.Shutdown();
        }
    }
}
