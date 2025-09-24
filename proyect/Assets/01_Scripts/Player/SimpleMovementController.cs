using UnityEngine;

public class SimpleMovementController : MonoBehaviour, IMovementController
{
    [Header("Movimiento")]
    public float velocidadMovimiento = 10.0f;
    public float velocidadRotacion = 300.0f;

    public void Move(float horizontal, float vertical, bool canMove)
    {
        // Debug siempre
        if (Mathf.Abs(horizontal) > 0.01f || Mathf.Abs(vertical) > 0.01f)
        {
            Debug.Log($"INPUT RECIBIDO: H={horizontal:F2}, V={vertical:F2}, canMove={canMove}");
        }

        if (!canMove)
        {
            Debug.Log("MOVIMIENTO BLOQUEADO por canMove=false");
            return;
        }

        // Aplicar rotación
        if (Mathf.Abs(horizontal) > 0.01f)
        {
            transform.Rotate(0, horizontal * velocidadRotacion * Time.deltaTime, 0);
        }

        // Aplicar movimiento directo
        if (Mathf.Abs(vertical) > 0.01f)
        {
            Vector3 movimiento = transform.forward * vertical * velocidadMovimiento * Time.deltaTime;
            transform.position += movimiento;
        }
    }
}


