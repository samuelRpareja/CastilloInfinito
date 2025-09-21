using UnityEngine;

public class GhostController : MonoBehaviour, IInitializable
{
    [SerializeField] private float aggroRange = 12f;
    [SerializeField] private float disengageRange = 15f; // reset si el player se aleja más

    private EnemyCommon enemy;
    private IEnemyAttack attack;
    private GhostPhase phase;
    private bool inited;
    private float initialHeight; // Altura inicial del fantasma

    private void Start() { if (!inited) Initialize(); }

    public void Initialize()
    {
        enemy  = GetComponent<EnemyCommon>();
        attack = GetComponent<IEnemyAttack>();
        phase  = GetComponent<GhostPhase>();

        if (enemy == null) { Debug.LogError("GhostController: falta EnemyCommon"); return; }

        // Guardar altura inicial del fantasma
        initialHeight = transform.position.y;

        // Ghost SIEMPRE volador
        enemy.movementMode = MovementMode.Flying;

        // Configurar movimiento solo horizontal (X y Z)
        enemy.verticalSpeedLimit = 0f;     // Sin movimiento vertical
        enemy.verticalAccel = 0f;          // Sin aceleración vertical
        
        // Reducir velocidad para que sea más lento
        enemy.moveSpeed = 2f;           // Más lento que el default (3.5f)
        enemy.accel = 8f;               // Aceleración más suave
        enemy.decel = 12f;              // Frenado más suave

        // Ajustes del CharacterController para volador
        var cc = GetComponent<CharacterController>();
        if (cc != null)
        {
            cc.slopeLimit = 90f;
            cc.stepOffset = 0f;
            cc.minMoveDistance = 0f;
            cc.skinWidth = Mathf.Max(0.02f, cc.skinWidth);
        }

        enemy.Initialize();
        enemy.fsm.Set(new IdleStateDebug(enemy, aggroRange));
        enemy.OnDeath += () => enemy.fsm.Set(new DeadState(enemy));
        inited = true;
    }

    private void Update()
    {
        if (!inited || enemy == null || enemy.IsDead) return;

        // El sistema de estados se encarga de la lógica de persecución
        // Solo manejamos el phase aleatorio aquí
        if (phase != null && phase.CanPhase && Random.value < 0.01f)
            phase.DoPhase();
            
        // FORZAR altura fija para evitar movimiento vertical
        ForceFixedHeight();
    }
    
    private void ForceFixedHeight()
    {
        // Mantener la altura inicial del fantasma
        Vector3 pos = transform.position;
        pos.y = initialHeight; // Altura fija en la altura inicial
        transform.position = pos;
    }

#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, aggroRange);
    }
#endif
}
