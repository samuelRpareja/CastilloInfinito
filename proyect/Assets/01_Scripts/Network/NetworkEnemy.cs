using UnityEngine;
using Unity.Netcode;

[RequireComponent(typeof(NetworkObject))]
public class NetworkEnemy : NetworkBehaviour
{
    [Header("Componentes del Enemigo")]
    [SerializeField] private Health enemyHealth;
    [SerializeField] private Animator enemyAnimator;
    [SerializeField] private Collider enemyCollider;
    
    [Header("Configuración de Red")]
    public NetworkVariable<Vector3> networkPosition = new NetworkVariable<Vector3>();
    public NetworkVariable<Quaternion> networkRotation = new NetworkVariable<Quaternion>();
    public NetworkVariable<bool> isAlive = new NetworkVariable<bool>(true);
    public NetworkVariable<float> currentHealth = new NetworkVariable<float>();
    
    [Header("Configuración de Sincronización")]
    public float syncRate = 20f; // Actualizaciones por segundo
    private float lastSyncTime;
    
    // Referencias locales
    private Vector3 lastPosition;
    private Quaternion lastRotation;
    
    private void Awake()
    {
        // Obtener componentes si no están asignados
        if (enemyHealth == null)
            enemyHealth = GetComponent<Health>();
        if (enemyAnimator == null)
            enemyAnimator = GetComponent<Animator>();
        if (enemyCollider == null)
            enemyCollider = GetComponent<Collider>();
    }
    
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        
        if (enemyHealth != null)
        {
            currentHealth.Value = enemyHealth.currentHP;
            enemyHealth.OnDeath += OnEnemyDeath;
            enemyHealth.OnHealthChanged += OnHealthChanged;
        }
    }
    
    private void Update()
    {
        if (IsServer)
        {
            // Solo el servidor actualiza las variables de red
            UpdateNetworkVariables();
        }
        else
        {
            // Los clientes interpolan hacia las posiciones de red
            InterpolateRemoteEnemy();
        }
    }
    
    private void UpdateNetworkVariables()
    {
        // Actualizar posición y rotación
        Vector3 currentPos = transform.position;
        Quaternion currentRot = transform.rotation;
        
        // Solo sincronizar si hay cambios significativos y ha pasado suficiente tiempo
        if (Time.time - lastSyncTime >= 1f / syncRate)
        {
            if (Vector3.Distance(currentPos, lastPosition) > 0.01f ||
                Quaternion.Angle(currentRot, lastRotation) > 1f)
            {
                networkPosition.Value = currentPos;
                networkRotation.Value = currentRot;
                lastSyncTime = Time.time;
            }
        }
        
        lastPosition = currentPos;
        lastRotation = currentRot;
    }
    
    private void InterpolateRemoteEnemy()
    {
        // Interpolar suavemente hacia la posición de red
        if (Vector3.Distance(transform.position, networkPosition.Value) > 0.1f)
        {
            transform.position = Vector3.Lerp(transform.position, networkPosition.Value, Time.deltaTime * 10f);
            transform.rotation = Quaternion.Lerp(transform.rotation, networkRotation.Value, Time.deltaTime * 10f);
        }
    }
    
    private void OnHealthChanged(float newHealth)
    {
        currentHealth.Value = newHealth;
        isAlive.Value = newHealth > 0f;
        
        // Sincronizar estado de vida
        if (IsServer)
        {
            UpdateHealthClientRpc(newHealth, isAlive.Value);
        }
    }
    
    [ClientRpc]
    private void UpdateHealthClientRpc(float health, bool alive)
    {
        if (enemyHealth != null)
        {
            enemyHealth.currentHP = health;
        }
        
        isAlive.Value = alive;
        
        // Actualizar animaciones
        if (enemyAnimator != null)
        {
            enemyAnimator.SetBool("IsAlive", alive);
        }
        
        // Deshabilitar colisiones si está muerto
        if (enemyCollider != null)
        {
            enemyCollider.enabled = alive;
        }
    }
    
    private void OnEnemyDeath()
    {
        if (IsServer)
        {
            isAlive.Value = false;
            OnEnemyDeathClientRpc();
        }
    }
    
    [ClientRpc]
    private void OnEnemyDeathClientRpc()
    {
        isAlive.Value = false;
        
        // Actualizar animaciones
        if (enemyAnimator != null)
        {
            enemyAnimator.SetTrigger("Death");
        }
        
        // Deshabilitar colisiones
        if (enemyCollider != null)
        {
            enemyCollider.enabled = false;
        }
        
        // Deshabilitar scripts de IA
        var aiScripts = GetComponents<MonoBehaviour>();
        foreach (var script in aiScripts)
        {
            if (script != this && script.GetType() != typeof(NetworkObject))
            {
                script.enabled = false;
            }
        }
    }
    
    // Método para sincronizar ataques
    [ServerRpc]
    public void TriggerAttackServerRpc()
    {
        TriggerAttackClientRpc();
    }
    
    [ClientRpc]
    private void TriggerAttackClientRpc()
    {
        if (enemyAnimator != null)
        {
            enemyAnimator.SetTrigger("Attack");
        }
    }
    
    // Método para sincronizar daño recibido
    [ServerRpc]
    public void TakeDamageServerRpc(float damage)
    {
        if (enemyHealth != null && isAlive.Value)
        {
            enemyHealth.TakeDamage(damage);
        }
    }
    
    // Método para sincronizar movimiento
    [ServerRpc]
    public void MoveToServerRpc(Vector3 position)
    {
        if (isAlive.Value)
        {
            networkPosition.Value = position;
            MoveToClientRpc(position);
        }
    }
    
    [ClientRpc]
    private void MoveToClientRpc(Vector3 position)
    {
        if (isAlive.Value)
        {
            transform.position = position;
        }
    }
    
    // Método para sincronizar rotación
    [ServerRpc]
    public void RotateToServerRpc(Quaternion rotation)
    {
        if (isAlive.Value)
        {
            networkRotation.Value = rotation;
            RotateToClientRpc(rotation);
        }
    }
    
    [ClientRpc]
    private void RotateToClientRpc(Quaternion rotation)
    {
        if (isAlive.Value)
        {
            transform.rotation = rotation;
        }
    }
    
    // Método para respawn del enemigo
    [ServerRpc]
    public void RespawnServerRpc()
    {
        if (enemyHealth != null)
        {
            enemyHealth.currentHP = enemyHealth.maxHP;
            isAlive.Value = true;
            RespawnClientRpc();
        }
    }
    
    [ClientRpc]
    private void RespawnClientRpc()
    {
        isAlive.Value = true;
        
        // Restaurar salud
        if (enemyHealth != null)
        {
            enemyHealth.currentHP = enemyHealth.maxHP;
        }
        
        // Restaurar colisiones
        if (enemyCollider != null)
        {
            enemyCollider.enabled = true;
        }
        
        // Restaurar scripts de IA
        var aiScripts = GetComponents<MonoBehaviour>();
        foreach (var script in aiScripts)
        {
            if (script != this && script.GetType() != typeof(NetworkObject))
            {
                script.enabled = true;
            }
        }
        
        // Actualizar animaciones
        if (enemyAnimator != null)
        {
            enemyAnimator.SetBool("IsAlive", true);
        }
    }
    
    public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn();
        Debug.Log($"👹 Enemigo sincronizado: {gameObject.name}");
    }
    
    public override void OnDestroy()
    {
        if (enemyHealth != null)
        {
            enemyHealth.OnDeath -= OnEnemyDeath;
            enemyHealth.OnHealthChanged -= OnHealthChanged;
        }
        base.OnDestroy();
    }
}
