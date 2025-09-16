using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostPhase : MonoBehaviour
{
    [Header("Phase")]
    [SerializeField] private float phaseTime = 0.8f;
    [SerializeField] private float cooldown = 4f;

    [Header("Layer Switch (opcional)")]
    [SerializeField] private string phaseLayerName = "GhostPhase";
    [SerializeField] private bool switchLayerDuringPhase = true;   // si no tienes la capa, ponlo en false

    [Header("Colisiones (opcional)")]
    [SerializeField] private bool disableControllerDuringPhase = false; // alternativa a cambiar de capa

    [Header("Visual")]
    [SerializeField] private float alphaWhilePhase = 0.4f;

    private float last;
    private int originalLayer;
    private int phaseLayerIndex = -1;
    private Renderer[] rends;
    private CharacterController cc;

    public bool CanPhase { get { return Time.time >= last + cooldown; } }

    private void Awake()
    {
        originalLayer = gameObject.layer;
        phaseLayerIndex = LayerMask.NameToLayer(phaseLayerName); // -1 si no existe
        rends = GetComponentsInChildren<Renderer>();
        cc = GetComponent<CharacterController>();
    }

    public void DoPhase()
    {
        if (!CanPhase) return;
        last = Time.time;
        StartCoroutine(PhaseCo());
    }

    private IEnumerator PhaseCo()
    {
        // Visual transparente
        SetAlpha(alphaWhilePhase);

        // Gestión de colisión:
        if (switchLayerDuringPhase && phaseLayerIndex != -1)
        {
            gameObject.layer = phaseLayerIndex;
        }
        else if (disableControllerDuringPhase && cc != null)
        {
            cc.detectCollisions = false; // evita chocar mientras phase
        }

        yield return new WaitForSeconds(phaseTime);

        // Restaurar
        SetAlpha(1f);

        if (switchLayerDuringPhase && phaseLayerIndex != -1)
        {
            gameObject.layer = originalLayer;
        }
        else if (disableControllerDuringPhase && cc != null)
        {
            cc.detectCollisions = true;
        }
    }

    private void SetAlpha(float a)
    {
        if (rends == null) return;
        for (int i = 0; i < rends.Length; i++)
        {
            var r = rends[i];
            if (r == null) continue;
            var mats = r.materials;
            for (int m = 0; m < mats.Length; m++)
            {
                var mat = mats[m];
                if (mat != null && mat.HasProperty("_Color"))
                {
                    var c = mat.color; c.a = a; mat.color = c;
                }
            }
        }
    }

}
