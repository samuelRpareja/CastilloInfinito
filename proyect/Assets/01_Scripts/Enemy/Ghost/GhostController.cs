using UnityEngine;

public class GhostController : MonoBehaviour, IInitializable
{
    [SerializeField] private float aggroRange = 12f;
    [SerializeField] private float disengageRange = 15f; // reset si el player se aleja más

    private EnemyCommon enemy;
    private IEnemyAttack attack;
    private GhostPhase phase;
    private bool inited;

    private void Start() { if (!inited) Initialize(); }

    public void Initialize()
    {
        enemy  = GetComponent<EnemyCommon>();
        attack = GetComponent<IEnemyAttack>();
        phase  = GetComponent<GhostPhase>();

        if (enemy == null) { Debug.LogError("GhostController: falta EnemyCommon"); return; }

        // Ghost SIEMPRE volador
        enemy.movementMode = MovementMode.Flying;

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
        enemy.fsm.Set(new IdleState(enemy, aggroRange));
        enemy.OnDeath += () => enemy.fsm.Set(new DeadState(enemy));
        inited = true;
    }

    private void Update()
    {
        // En GhostController.cs, dentro del método Update()

        if (attack != null && attack.CanAttack())
        {
            // AÑADE ESTA LÍNEA
           
            enemy.fsm.Set(new AttackState(enemy, aggroRange, attack));
        }

        if (!inited || enemy == null || enemy.IsDead) return;
        if (attack != null && attack.CanAttack())
        {
            enemy.fsm.Set(new AttackState(enemy, aggroRange, attack));
        }
        var tgt = enemy.Target;
        if (tgt == null || !tgt.IsValid)
        {
            enemy.Halt(); // quieto de verdad en idle
            enemy.fsm.Set(new IdleState(enemy, aggroRange));
            return;
        }

        float dist = Vector3.Distance(enemy.transform.position, tgt.AimRoot.position);

        if (dist <= aggroRange)
        {
            Vector3 dir = tgt.AimRoot.position - enemy.transform.position;
            enemy.MoveTowards(dir, Time.deltaTime, true);

            if (attack != null && attack.CanAttack())
                enemy.fsm.Set(new AttackState(enemy, aggroRange, attack));
        }
        else if (dist > disengageRange)
        {
            enemy.Halt();
            enemy.fsm.Set(new IdleState(enemy, aggroRange));
        }


        if (!inited || enemy == null || enemy.IsDead) return;

        // si hay target y puede atacar → AttackState
        if (attack != null && attack.CanAttack())
            enemy.fsm.Set(new AttackState(enemy, aggroRange, attack));

        if (phase != null && phase.CanPhase && Random.value < 0.01f)
            phase.DoPhase();

        // Descomenta si quieres phase aleatorio (puede causar microcambios de colisión)
        // if (phase != null && phase.CanPhase && Random.value < 0.01f)
        //     phase.DoPhase();
    }

#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, aggroRange);
    }
#endif
}
