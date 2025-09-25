using UnityEngine;

/// <summary>
/// Script para mostrar la vida del jugador con el mismo formato que TargetDummy
/// </summary>
public class PlayerDeathTester : MonoBehaviour
{
    private PlayerProxy playerProxy;
    private float lastDamageAmount = 0f;
    private float damageDisplayTime = 2f; // Tiempo en segundos para mostrar el daño
    private float lastDamageTime = 0f;
    
    void Start()
    {
        // Buscar PlayerProxy
        playerProxy = FindObjectOfType<PlayerProxy>();
        if (playerProxy == null)
        {
            Debug.LogError("PlayerDeathTester: No se encontró PlayerProxy en la escena");
            return;
        }
        
        Debug.Log($"✅ PlayerDeathTester iniciado - HP actual: {playerProxy.CurrentHP}/{playerProxy.MaxHP}");
    }
    
    void Update()
    {
        if (playerProxy == null) return;
        
        // Resetear el daño después del tiempo especificado
        if (lastDamageAmount > 0f && Time.time - lastDamageTime > damageDisplayTime)
        {
            lastDamageAmount = 0f;
        }
    }
    
    void OnGUI()
    {
        if (playerProxy == null) return;
        
        // Mostrar UI de vida en pantalla
        GUILayout.BeginArea(new Rect(10, 10, 300, 120));
        GUILayout.Label("📊 VIDA DEL JUGADOR", GUI.skin.box);
        
        // Mostrar formato igual al TargetDummy si hay daño reciente
        if (lastDamageAmount > 0f)
        {
            GUILayout.Label($"[Player] Daño {lastDamageAmount:F0} → HP: {playerProxy.CurrentHP:F0}/{playerProxy.MaxHP:F0}");
        }
        else
        {
            GUILayout.Label($"HP: {playerProxy.CurrentHP:F0}/{playerProxy.MaxHP:F0}");
        }
        
        GUILayout.Label($"Estado: {(playerProxy.IsDead ? "MUERTO" : "VIVO")}");
        GUILayout.EndArea();
    }
    
    // Método público para registrar daño (llamado desde otros scripts)
    public void RegisterDamage(float damageAmount)
    {
        lastDamageAmount = damageAmount;
        lastDamageTime = Time.time;
    }
}
