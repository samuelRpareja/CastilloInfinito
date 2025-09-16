using System.Collections;
using UnityEngine;

public class SimpleAttacker : MonoBehaviour, IAttacker
{
    [Header("Golpe")]
    public float duracionGolpe = 1.0f;

    public bool IsAttacking { get; private set; }

    public System.Action OnAttackStarted;
    public System.Action OnAttackEnded;

    public void TryAttack()
    {
        if (IsAttacking)
        {
            return;
        }

        IsAttacking = true;
        OnAttackStarted?.Invoke();
        StartCoroutine(ResetAttackRoutine());
    }

    private IEnumerator ResetAttackRoutine()
    {
        yield return new WaitForSeconds(duracionGolpe);
        IsAttacking = false;
        OnAttackEnded?.Invoke();
    }
}


