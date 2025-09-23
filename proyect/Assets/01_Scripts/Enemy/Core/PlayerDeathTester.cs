using UnityEngine;

/// <summary>
/// Script para testing del sistema de muerte del player
/// Agregar a cualquier GameObject para probar la funcionalidad
/// </summary>
public class PlayerDeathTester : MonoBehaviour
{
    [Header("Testing Controls")]
    [SerializeField] private KeyCode testDeathKey = KeyCode.T;
    [SerializeField] private KeyCode damagePlayerKey = KeyCode.D;
    [SerializeField] private KeyCode healPlayerKey = KeyCode.H;
    [SerializeField] private float damageAmount = 25f;
    [SerializeField] private float healAmount = 50f;
    
    [Header("Auto Setup")]
    [SerializeField] private bool autoSetupGameOver = true;
    [SerializeField] private float restartDelay = 3f;
    
    private PlayerProxy playerProxy;
    private SimpleGameOver gameOverSystem;
    
    void Start()
    {
        // Buscar PlayerProxy
        playerProxy = FindObjectOfType<PlayerProxy>();
        if (playerProxy == null)
        {
            Debug.LogError("PlayerDeathTester: No se encontró PlayerProxy en la escena");
            return;
        }
        
        // Configurar Game Over automáticamente si está habilitado
        if (autoSetupGameOver)
        {
            SetupGameOverSystem();
        }
        
        // Mostrar instrucciones
        ShowInstructions();
    }
    
    void Update()
    {
        if (playerProxy == null) return;
        
        // Testing con teclas
        if (Input.GetKeyDown(testDeathKey))
        {
            TestPlayerDeath();
        }
        
        if (Input.GetKeyDown(damagePlayerKey))
        {
            DamagePlayer();
        }
        
        if (Input.GetKeyDown(healPlayerKey))
        {
            HealPlayer();
        }
    }
    
    void SetupGameOverSystem()
    {
        // Verificar si ya existe un sistema de Game Over
        gameOverSystem = FindObjectOfType<SimpleGameOver>();
        if (gameOverSystem == null)
        {
            // Crear GameOverManager automáticamente
            GameObject gameOverObject = new GameObject("GameOverManager");
            gameOverSystem = gameOverObject.AddComponent<SimpleGameOver>();
            gameOverSystem.restartDelay = restartDelay;
            
            Debug.Log("✅ Sistema de Game Over configurado automáticamente");
        }
        else
        {
            Debug.Log("✅ Sistema de Game Over ya existe");
        }
    }
    
    void ShowInstructions()
    {
        Debug.Log("🎮 === CONTROLES DE TESTING ===");
        Debug.Log($"   {testDeathKey} - Matar player instantáneamente");
        Debug.Log($"   {damagePlayerKey} - Hacer daño al player ({damageAmount})");
        Debug.Log($"   {healPlayerKey} - Curar player ({healAmount})");
        Debug.Log($"   HP actual: {playerProxy.CurrentHP}/{playerProxy.MaxHP}");
        Debug.Log("================================");
    }
    
    [ContextMenu("Test Player Death")]
    public void TestPlayerDeath()
    {
        if (playerProxy == null)
        {
            Debug.LogError("❌ PlayerProxy no encontrado");
            return;
        }
        
        Debug.LogWarning("💀 TESTING: Matando player instantáneamente...");
        playerProxy.TakeDamage(playerProxy.CurrentHP);
    }
    
    [ContextMenu("Damage Player")]
    public void DamagePlayer()
    {
        if (playerProxy == null)
        {
            Debug.LogError("❌ PlayerProxy no encontrado");
            return;
        }
        
        Debug.Log($"⚔️ TESTING: Haciendo {damageAmount} de daño al player");
        playerProxy.TakeDamage(damageAmount);
        Debug.Log($"   HP actual: {playerProxy.CurrentHP}/{playerProxy.MaxHP}");
    }
    
    [ContextMenu("Heal Player")]
    public void HealPlayer()
    {
        if (playerProxy == null)
        {
            Debug.LogError("❌ PlayerProxy no encontrado");
            return;
        }
        
        // Simular curación (aumentar HP)
        float newHP = Mathf.Min(playerProxy.CurrentHP + healAmount, playerProxy.MaxHP);
        float healAmountActual = newHP - playerProxy.CurrentHP;
        
        Debug.Log($"❤️ TESTING: Curando {healAmountActual} HP al player");
        
        // Usar reflexión para modificar HP directamente (solo para testing)
        var hpField = typeof(PlayerProxy).GetField("_hp", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        if (hpField != null)
        {
            hpField.SetValue(playerProxy, newHP);
            Debug.Log($"   HP actual: {playerProxy.CurrentHP}/{playerProxy.MaxHP}");
        }
    }
    
    [ContextMenu("Show Player Status")]
    public void ShowPlayerStatus()
    {
        if (playerProxy == null)
        {
            Debug.LogError("❌ PlayerProxy no encontrado");
            return;
        }
        
        Debug.Log("📊 === ESTADO DEL PLAYER ===");
        Debug.Log($"   HP: {playerProxy.CurrentHP}/{playerProxy.MaxHP}");
        Debug.Log($"   Está muerto: {playerProxy.IsDead}");
        Debug.Log($"   Es válido: {playerProxy.IsValid}");
        Debug.Log($"   Posición: {playerProxy.transform.position}");
        Debug.Log("============================");
    }
    
    void OnGUI()
    {
        if (playerProxy == null) return;
        
        // Mostrar UI de testing en pantalla
        GUILayout.BeginArea(new Rect(10, 10, 300, 200));
        GUILayout.Label("🎮 CONTROLES DE TESTING", GUI.skin.box);
        GUILayout.Label($"HP: {playerProxy.CurrentHP:F1}/{playerProxy.MaxHP:F1}");
        GUILayout.Label($"Estado: {(playerProxy.IsDead ? "MUERTO" : "VIVO")}");
        GUILayout.Space(10);
        
        if (GUILayout.Button($"Matar Player ({testDeathKey})"))
        {
            TestPlayerDeath();
        }
        
        if (GUILayout.Button($"Daño ({damagePlayerKey})"))
        {
            DamagePlayer();
        }
        
        if (GUILayout.Button($"Curar ({healPlayerKey})"))
        {
            HealPlayer();
        }
        
        GUILayout.EndArea();
    }
}
