using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Script que maneja el daño de los ataques del personaje a los fantasmas
/// </summary>
public class PlayerAttackDamage : MonoBehaviour
{
    [Header("Configuración de Ataque")]
    [SerializeField] private float attackDamage = 10f;
    [SerializeField] private float attackRange = 2f;
    [SerializeField] private float attackAngle = 120f; // Ángulo de ataque en grados
    [SerializeField] private LayerMask enemyLayerMask = -1; // Capa de enemigos
    
    [Header("Debug")]
    [SerializeField] private bool showDebugGizmos = true;
    [SerializeField] private Color debugColor = Color.red;
    
    private SimpleAttacker attacker;
    private Transform playerTransform;
    private List<EnemyCommon> enemiesHitThisAttack = new List<EnemyCommon>();
    
    void Start()
    {
        attacker = GetComponent<SimpleAttacker>();
        playerTransform = transform;
        
        if (attacker == null)
        {
            Debug.LogError("PlayerAttackDamage: No se encontró SimpleAttacker en el personaje");
            return;
        }
        
        // Suscribirse a los eventos de ataque
        attacker.OnAttackStarted += OnAttackStarted;
        attacker.OnAttackEnded += OnAttackEnded;
        
        Debug.Log("PlayerAttackDamage: Sistema de daño inicializado");
    }
    
    void OnDestroy()
    {
        // Desuscribirse de los eventos
        if (attacker != null)
        {
            attacker.OnAttackStarted -= OnAttackStarted;
            attacker.OnAttackEnded -= OnAttackEnded;
        }
    }
    
    void OnAttackStarted()
    {
        Debug.Log("PlayerAttackDamage: Ataque iniciado - Buscando enemigos cercanos");
        enemiesHitThisAttack.Clear();
        DamageEnemiesInRange();
    }
    
    void OnAttackEnded()
    {
        Debug.Log($"PlayerAttackDamage: Ataque terminado - {enemiesHitThisAttack.Count} enemigos golpeados");
        enemiesHitThisAttack.Clear();
    }
    
    void DamageEnemiesInRange()
    {
        // Buscar todos los enemigos en el rango
        Collider[] enemiesInRange = Physics.OverlapSphere(playerTransform.position, attackRange, enemyLayerMask);
        
        foreach (Collider enemyCollider in enemiesInRange)
        {
            // Verificar si está dentro del ángulo de ataque
            if (IsEnemyInAttackAngle(enemyCollider.transform))
            {
                // Obtener el componente EnemyCommon
                EnemyCommon enemy = enemyCollider.GetComponent<EnemyCommon>();
                if (enemy != null && !enemiesHitThisAttack.Contains(enemy))
                {
                    // Aplicar daño
                    enemy.TakeDamage(attackDamage);
                    enemiesHitThisAttack.Add(enemy);
                    
                    Debug.Log($"PlayerAttackDamage: Golpeó a {enemy.name} por {attackDamage} de daño");
                }
            }
        }
    }
    
    bool IsEnemyInAttackAngle(Transform enemyTransform)
    {
        // Calcular dirección hacia el enemigo
        Vector3 directionToEnemy = (enemyTransform.position - playerTransform.position).normalized;
        Vector3 playerForward = playerTransform.forward;
        
        // Calcular ángulo entre la dirección del jugador y la dirección al enemigo
        float angle = Vector3.Angle(playerForward, directionToEnemy);
        
        // Verificar si está dentro del ángulo de ataque
        return angle <= attackAngle / 2f;
    }
    
    void OnDrawGizmos()
    {
        if (!showDebugGizmos || playerTransform == null) return;
        
        // Dibujar rango de ataque
        Gizmos.color = debugColor;
        Gizmos.DrawWireSphere(playerTransform.position, attackRange);
        
        // Dibujar ángulo de ataque
        Gizmos.color = new Color(debugColor.r, debugColor.g, debugColor.b, 0.3f);
        Vector3 leftBoundary = Quaternion.AngleAxis(-attackAngle / 2f, Vector3.up) * playerTransform.forward * attackRange;
        Vector3 rightBoundary = Quaternion.AngleAxis(attackAngle / 2f, Vector3.up) * playerTransform.forward * attackRange;
        
        Gizmos.DrawLine(playerTransform.position, playerTransform.position + leftBoundary);
        Gizmos.DrawLine(playerTransform.position, playerTransform.position + rightBoundary);
        Gizmos.DrawLine(playerTransform.position + leftBoundary, playerTransform.position + rightBoundary);
    }
    
    void OnDrawGizmosSelected()
    {
        if (playerTransform == null) return;
        
        // Dibujar información adicional cuando está seleccionado
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(playerTransform.position, attackRange);
        
        // Mostrar dirección de ataque
        Gizmos.color = Color.green;
        Gizmos.DrawRay(playerTransform.position, playerTransform.forward * attackRange);
    }
}
