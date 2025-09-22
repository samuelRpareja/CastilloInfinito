using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetRegistry : MonoBehaviour
{
    public static TargetRegistry Instance { get; private set; }
    public ITarget CurrentTarget { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    public void Register(ITarget target) 
    {
        CurrentTarget = target;
        Debug.Log($"TargetRegistry: Target registrado - {target.GetType().Name} (IsValid: {target.IsValid})");
    }

    // dentro de TargetRegistry.cs (tu singleton)
    public void SetTarget(ITarget t)
    {
        CurrentTarget = t;
        Debug.Log($"TargetRegistry: Target establecido - {t?.GetType().Name} (IsValid: {t?.IsValid})");
    }


    public void Unregister(ITarget target)
    {
        if (CurrentTarget == target) CurrentTarget = null;
    }
}
