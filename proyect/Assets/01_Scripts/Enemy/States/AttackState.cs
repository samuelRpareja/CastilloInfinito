using UnityEngine;

// Asegúrate de que tu estado implemente una interfaz base como IState
public class AttackState : IState
{
    private readonly EnemyCommon _enemy;
    private readonly IEnemyAttack _attack;
    private readonly float _duration;
    private float _timer;

    // El constructor recibe todo lo que necesita
    public AttackState(EnemyCommon enemy, float aggroRange, IEnemyAttack attack)
    {
        _enemy = enemy;
        _attack = attack;
        // Calculamos cuánto tiempo debemos permanecer en este estado
    }

    // En AttackState.cs

    // En AttackState.cs

    public void OnEnter()
    {
        // Puedes dejar este como estaba o incluso borrarlo después
        // Lo importante es que la lógica esté en Enter()
    }

    public void Tick(float dt)
    {
        // ... tu lógica de Tick ...
    }

    public void OnExit() { }

    // 👇👇 MUEVE TODA LA LÓGICA AQUÍ 👇👇
    public void Enter()
    {
        Debug.LogWarning($"<color=magenta>[AttackState] Enter() - {_enemy.name}</color>");
        Debug.Log($"   🎯 Iniciando ataque a las {Time.time:F2}s");

        if (_attack != null)
        {
            Debug.LogWarning("   ✅ La referencia 'attack' NO es nula. Se llamará a DoAttack() AHORA.");
            Debug.Log($"   🎯 Tipo de ataque: {_attack.GetType().Name}");
            Debug.Log($"   ⏰ Cooldown: {_attack.Cooldown:F2}s");
            
            _attack.DoAttack();

            _timer = Time.time + Mathf.Max(0.5f, _attack.Cooldown * 0.6f);
            Debug.Log($"   ⏱️ Timer configurado para {_timer:F2}s");
        }
        else
        {
            Debug.LogError("   ❌ ¡ERROR! La referencia 'attack' ES NULA. No se puede llamar a DoAttack().");
            _timer = Time.time + 0.4f;
        }
    }

    public void Exit()
    {
        // Si Enter() funciona, probablemente necesites mover la lógica
        // de OnExit() aquí también.
    }
}