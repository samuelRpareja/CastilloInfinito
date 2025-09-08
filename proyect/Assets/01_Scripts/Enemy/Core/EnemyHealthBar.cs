using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



/// <summary>
/// Barra de vida world-space para enemigos. Lee HP vía IHealthReadable
/// y se auto-orienta hacia la cámara (billboard).
/// </summary>
public class EnemyHealthBar : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private Slider slider;
    [SerializeField] private Transform billboardTarget;   // si es null, usa Camera.main
    [SerializeField] private Component healthSource;      // opcional: referencia directa a un comp. que implemente IHealthReadable

    private IHealthReadable _health;
    private Camera _cam;

    private void Awake()
    {
        if (slider == null)
        {
            slider = GetComponentInChildren<Slider>();
            if (slider == null)
                Debug.LogWarning($"{name}: EnemyHealthBar no encontró Slider.");
        }

        // Resolver fuente de salud
        if (healthSource != null)
        {
            _health = healthSource as IHealthReadable;
        }
        if (_health == null)
        {
            // Busca hacia arriba (padres) un IHealthReadable (p. ej., EnemyCommon)
            _health = GetComponentInParent<IHealthReadable>();
        }

        if (_health == null)
        {
            Debug.LogWarning($"{name}: EnemyHealthBar no encontró IHealthReadable en padres ni en 'healthSource'.");
        }
    }

    private void OnEnable()
    {
        _cam = Camera.main;
        if (_health != null)
            _health.OnHealthChanged += OnHealthChanged;
        // Inicializa valor si ya tenemos salud disponible
        if (_health != null && slider != null)
            slider.value = Mathf.Clamp01(_health.CurrentHP / Mathf.Max(1f, _health.MaxHP));
    }

    private void OnDisable()
    {
        if (_health != null)
            _health.OnHealthChanged -= OnHealthChanged;
    }

    private void LateUpdate()
    {
        // Billboard simple
        var target = billboardTarget ? billboardTarget : (_cam ? _cam.transform : null);
        if (target != null)
        {
            transform.forward = (transform.position - target.position).normalized;
        }
    }

    private void OnHealthChanged(float current, float max)
    {
        if (slider == null) return;
        slider.value = max > 0.0001f ? Mathf.Clamp01(current / max) : 0f;
    }

}