using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostAnimationBridge : MonoBehaviour
{

    [Header("Animator params (deben existir en tu Animator)")]
    [SerializeField] private string isMovingParam = "isMoving";
    [SerializeField] private string attackTrigger = "attack";         // o "attack_shift" si prefieres
    [SerializeField] private string surprisedTrigger = "surprised";
    [SerializeField] private string dissolveTrigger = "dissolve";

    [Header("Movimiento")]
    [SerializeField] private float movingThreshold = 0.05f; // umbral para considerar que se mueve

    private Animator anim;
    private EnemyCommon enemy;
    private Vector3 lastPos;
    private float lastHP = -1f;

    private int hIsMoving, hAttack, hSurprised, hDissolve;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        if (anim) anim.applyRootMotion = false; // <- clav

        anim = GetComponent<Animator>();
        enemy = GetComponent<EnemyCommon>();

        hIsMoving = Animator.StringToHash(isMovingParam);
        hAttack = Animator.StringToHash(attackTrigger);
        hSurprised = Animator.StringToHash(surprisedTrigger);
        hDissolve = Animator.StringToHash(dissolveTrigger);
    }

    private void OnEnable()
    {
        lastPos = transform.position;

        // nos suscribimos para detectar daño y muerte
        if (enemy != null)
        {
            enemy.OnDeath += OnDeath;
            // Si tu EnemyCommon expone OnHealthChanged, lo usamos para "surprised"
            enemy.OnHealthChanged += OnHealthChanged;
            lastHP = enemy.CurrentHP;
        }
    }

    private void OnDisable()
    {
        if (enemy != null)
        {
            enemy.OnDeath -= OnDeath;
            enemy.OnHealthChanged -= OnHealthChanged;
        }
    }

    private void Update()
    {
        // Calcula velocidad horizontal y setea isMoving (usas bool, no blend)
        Vector3 delta = transform.position - lastPos; delta.y = 0f;
        float speed = delta.magnitude / Mathf.Max(Time.deltaTime, 0.0001f);
        bool isMoving = speed > movingThreshold;
        anim.SetBool(hIsMoving, isMoving);
        lastPos = transform.position;
    }

    private void OnHealthChanged(float current, float max)
    {
        // si bajó vida respecto al último valor → surprised
        if (lastHP < 0f) { lastHP = current; return; }
        if (current < lastHP)
        {
            anim.SetTrigger(hSurprised);
        }
        lastHP = current;
    }

    private void OnDeath()
    {
        anim.SetTrigger(hDissolve);
    }

    // === API pública para que los ataques disparen animaciones ===
    public void PlayAttack() => anim.SetTrigger(hAttack);
    public void PlayAttackShift() => anim.SetTrigger(hAttack); // si usas otro trigger, cámbialo aquí
    public void PlaySurprised() => anim.SetTrigger(hSurprised);
    public void PlayDissolve() => anim.SetTrigger(hDissolve);
}