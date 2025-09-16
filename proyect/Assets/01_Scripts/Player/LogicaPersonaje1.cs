using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LogicaPersonaje1 : MonoBehaviour
{
    [Header("Movimiento")]
    public float velocidadMovimiento = 5.0f;
    public float velocidadRotacion = 200.0f;
    
    [Header("Golpe")]
    public bool estoyAtacando = false;
    public float duracionGolpe = 1.0f; // Duración del golpe en segundos
    
    private Animator anim;
    private float x, y;

    void Start()
    {
        anim = GetComponent<Animator>();
    }

    void FixedUpdate()
    {
        // Movimiento solo si no está atacando
        if (!estoyAtacando)
        {
            transform.Rotate(0, x * Time.deltaTime * velocidadRotacion, 0);
            transform.Translate(0, 0, y * Time.deltaTime * velocidadMovimiento);
        }
    }

    void Update()
    {
        // Input de movimiento
        x = Input.GetAxis("Horizontal");
        y = Input.GetAxis("Vertical");
        
        // Actualizar animaciones de movimiento
        if (anim != null)
        {
            anim.SetFloat("VelX", x);
            anim.SetFloat("VelY", y);
        }
        
        // Detectar golpe con Enter
        if (Input.GetKeyDown(KeyCode.Return) && !estoyAtacando)
        {
            anim.SetTrigger("golpeo");
            estoyAtacando = true;
            StartCoroutine(ResetearGolpe());
        }
    }

    public void DejeDeGolpear()
    {
        estoyAtacando = false;
    }
    
    // Corrutina para resetear automáticamente el golpe
    IEnumerator ResetearGolpe()
    {
        yield return new WaitForSeconds(duracionGolpe);
        estoyAtacando = false;
        Debug.Log("Golpe terminado - Puede moverse de nuevo");
    }
}
