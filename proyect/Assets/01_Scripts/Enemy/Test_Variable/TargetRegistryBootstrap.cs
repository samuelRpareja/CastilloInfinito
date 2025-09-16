using UnityEngine;

public class TargetRegistryBootstrap : MonoBehaviour
{
    void Awake()
    {
        var dummy = FindObjectOfType<TargetDummy>();
        if (dummy == null)
        {
            Debug.LogWarning("[Bootstrap] No se encontró TargetDummy en la escena.");
            return;
        }

        if (TargetRegistry.Instance == null)
        {
            var go = new GameObject("TargetRegistry");
            go.AddComponent<TargetRegistry>();
        }

        TargetRegistry.Instance.SetTarget(dummy);
        Debug.Log("[Bootstrap] Target configurado → " + dummy.name);
    }
}
