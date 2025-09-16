using UnityEngine;

[DisallowMultipleComponent]
public class GhostAnimationBridgeExternal : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private Animator anim;          // Asigna el Animator del hijo (FBX)
    [SerializeField] private EnemyCommon enemyRoot;  // Si es null, se busca en este GO

    [Header("Animator params")]
    [SerializeField] private string isMovingParam = "isMoving";
    [SerializeField] private string attackTrigger = "attack";
    [SerializeField] private string surprisedTrigger = "surprised";
    [SerializeField] private string dissolveTrigger = "dissolve";

    [Header("Movimiento")]
    [SerializeField] private float movingThreshold = 0.05f;

    private Vector3 lastPos;
    private float lastHP = -1f;

    private int hIsMoving, hAttack, hSurprised, hDissolve;

    private void Reset()
    {
        if (anim == null) anim = GetComponentInChildren<Animator>();
        if (enemyRoot == null) enemyRoot = GetComponent<EnemyCommon>();
    }

    private void Awake()
    {
        if (enemyRoot == null) enemyRoot = GetComponent<EnemyCommon>();
        if (anim == null)
            Debug.LogWarning($"{name}: Asigna el Animator del hijo al campo 'anim'.");

        hIsMoving = Animator.StringToHash(isMovingParam);
        hAttack = Animator.StringToHash(attackTrigger);
        hSurprised = Animator.StringToHash(surprisedTrigger);
        hDissolve = Animator.StringToHash(dissolveTrigger);
    }

    private void OnEnable()
    {
        lastPos = transform.position;

        if (enemyRoot != null)
        {
            enemyRoot.OnDeath += OnDeath;
            enemyRoot.OnHealthChanged += OnHealthChanged;
            lastHP = enemyRoot.CurrentHP;
        }
    }

    private void OnDisable()
    {
        if (enemyRoot != null)
        {
            enemyRoot.OnDeath -= OnDeath;
            enemyRoot.OnHealthChanged -= OnHealthChanged;
        }
    }

    private void Update()
    {
        if (anim == null) return;

        // calcula si se mueve (bool isMoving)
        Vector3 delta = transform.position - lastPos; delta.y = 0f;
        bool moving = (delta.magnitude / Mathf.Max(Time.deltaTime, 0.0001f)) > movingThreshold;
        anim.SetBool(hIsMoving, moving);
        lastPos = transform.position;
    }

    private void OnHealthChanged(float current, float max)
    {
        if (anim == null) return;
        if (lastHP >= 0f && current < lastHP) anim.SetTrigger(hSurprised);
        lastHP = current;
    }

    private void OnDeath()
    {
        if (anim != null) anim.SetTrigger(hDissolve);
    }

    // === API pública para ataques / testers ===
    public void PlayAttack() { if (anim != null) anim.SetTrigger(hAttack); }
    public void PlaySurprised() { if (anim != null) anim.SetTrigger(hSurprised); }
    public void PlayDissolve() { if (anim != null) anim.SetTrigger(hDissolve); }
}
