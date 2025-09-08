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

    public void Register(ITarget target) => CurrentTarget = target;

    public void Unregister(ITarget target)
    {
        if (CurrentTarget == target) CurrentTarget = null;
    }
}
