using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Netcode;

public class NetworkGameManager : NetworkBehaviour
{
    [Header("Configuración del Juego")]
    public float restartDelay = 2f;
    public GameObject deathUI;
    
    [Header("Configuración de Red")]
    public NetworkVariable<bool> isGameOver = new NetworkVariable<bool>(false);
    public NetworkVariable<int> alivePlayers = new NetworkVariable<int>(0);
    
    private float restartTimer = 0f;
    private NetworkManager networkManager;
    
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        
        networkManager = FindObjectOfType<NetworkManager>();
        
        // Suscribirse a eventos de red
        if (IsServer)
        {
            Unity.Netcode.NetworkManager.Singleton.OnClientConnectedCallback += OnPlayerConnected;
            Unity.Netcode.NetworkManager.Singleton.OnClientDisconnectCallback += OnPlayerDisconnected;
        }
        
        // Contar jugadores vivos iniciales
        CountAlivePlayers();
    }
    
    private void Update()
    {
        if (IsServer)
        {
            // Solo el servidor maneja la lógica del juego
            HandleGameLogic();
        }
    }
    
    private void HandleGameLogic()
    {
        // Contar jugadores vivos
        CountAlivePlayers();
        
        // Si no hay jugadores vivos, iniciar game over
        if (alivePlayers.Value == 0 && !isGameOver.Value)
        {
            StartGameOver();
        }
        
        // Si el juego terminó, contar hacia el reinicio
        if (isGameOver.Value)
        {
            restartTimer += Time.deltaTime;
            
            // Mostrar tiempo restante
            if (restartTimer % 1f < Time.deltaTime)
            {
                float timeLeft = restartDelay - restartTimer;
                if (timeLeft > 0)
                {
                    Debug.Log($"💀 Reiniciando en {timeLeft:F0} segundos...");
                    ShowRestartTimeClientRpc(timeLeft);
                }
            }
            
            // Reiniciar cuando se acabe el tiempo
            if (restartTimer >= restartDelay)
            {
                RestartGame();
            }
        }
    }
    
    private void CountAlivePlayers()
    {
        int count = 0;
        
        // Buscar todos los jugadores en la escena
        var players = FindObjectsOfType<NetworkPlayer>();
        foreach (var player in players)
        {
            if (player != null && player.GetComponent<Health>() != null && player.GetComponent<Health>().IsAlive())
            {
                count++;
            }
        }
        
        alivePlayers.Value = count;
    }
    
    private void StartGameOver()
    {
        isGameOver.Value = true;
        restartTimer = 0f;
        
        Debug.Log("💀 ¡GAME OVER! Todos los jugadores han muerto.");
        
        // Notificar a todos los clientes
        ShowGameOverClientRpc();
    }
    
    [ClientRpc]
    private void ShowGameOverClientRpc()
    {
        Debug.Log("💀 ¡GAME OVER! La partida se reiniciará en " + restartDelay + " segundos...");
        
        // Mostrar UI de muerte si está configurada
        if (deathUI != null)
        {
            deathUI.SetActive(true);
        }
        
        // Ralentizar el tiempo
        Time.timeScale = 0.5f;
    }
    
    [ClientRpc]
    private void ShowRestartTimeClientRpc(float timeLeft)
    {
        Debug.Log($"💀 Reiniciando en {timeLeft:F0} segundos...");
    }
    
    private void RestartGame()
    {
        Debug.Log("🔄 Reiniciando partida...");
        
        // Restaurar velocidad normal
        Time.timeScale = 1f;
        
        // Reiniciar la escena
        RestartSceneClientRpc();
    }
    
    [ClientRpc]
    private void RestartSceneClientRpc()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    
    private void OnPlayerConnected(ulong clientId)
    {
        Debug.Log($"👤 Jugador conectado: {clientId}");
        
        // Suscribirse a la muerte del nuevo jugador
        if (Unity.Netcode.NetworkManager.Singleton != null)
        {
            var client = Unity.Netcode.NetworkManager.Singleton.ConnectedClients[clientId];
            if (client.PlayerObject != null)
            {
                var player = client.PlayerObject.GetComponent<NetworkPlayer>();
                if (player != null)
                {
                    var health = player.GetComponent<Health>();
                    if (health != null)
                    {
                        health.OnDeath += OnPlayerDeath;
                    }
                }
            }
        }
    }
    
    private void OnPlayerDisconnected(ulong clientId)
    {
        Debug.Log($"👋 Jugador desconectado: {clientId}");
        
        // Desuscribirse de eventos del jugador desconectado
        if (Unity.Netcode.NetworkManager.Singleton != null)
        {
            var client = Unity.Netcode.NetworkManager.Singleton.ConnectedClients[clientId];
            if (client.PlayerObject != null)
            {
                var player = client.PlayerObject.GetComponent<NetworkPlayer>();
                if (player != null)
                {
                    var health = player.GetComponent<Health>();
                    if (health != null)
                    {
                        health.OnDeath -= OnPlayerDeath;
                    }
                }
            }
        }
    }
    
    private void OnPlayerDeath()
    {
        Debug.Log("💀 Un jugador ha muerto");
        
        // El servidor ya cuenta los jugadores vivos en Update()
        // No necesitamos hacer nada más aquí
    }
    
    // Método para revivir a todos los jugadores
    [ServerRpc]
    public void ReviveAllPlayersServerRpc()
    {
        ReviveAllPlayersClientRpc();
    }
    
    [ClientRpc]
    private void ReviveAllPlayersClientRpc()
    {
        var players = FindObjectsOfType<NetworkPlayer>();
        foreach (var player in players)
        {
            var health = player.GetComponent<Health>();
            if (health != null)
            {
                health.Heal(health.maxHP);
            }
        }
        
        // Restaurar velocidad normal
        Time.timeScale = 1f;
        
        // Ocultar UI de muerte
        if (deathUI != null)
        {
            deathUI.SetActive(false);
        }
    }
    
    // Método para reiniciar inmediatamente
    [ServerRpc]
    public void RestartImmediatelyServerRpc()
    {
        RestartGame();
    }
    
    public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn();
        Debug.Log("🎮 NetworkGameManager detenido");
    }
    
    private void OnDestroy()
    {
        // Limpiar suscripciones
        if (IsServer && Unity.Netcode.NetworkManager.Singleton != null)
        {
            Unity.Netcode.NetworkManager.Singleton.OnClientConnectedCallback -= OnPlayerConnected;
            Unity.Netcode.NetworkManager.Singleton.OnClientDisconnectCallback -= OnPlayerDisconnected;
        }
        
        // Restaurar velocidad normal
        Time.timeScale = 1f;
    }
}
