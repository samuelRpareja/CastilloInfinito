using UnityEngine;

[RequireComponent(typeof(Transform))]
public class SimpleMovementController : MonoBehaviour, IMovementController
{
    [Header("Movimiento")]
    public float velocidadMovimiento = 5.0f;
    public float velocidadRotacion = 200.0f;

    public void Move(float horizontal, float vertical, bool canMove)
    {
        if (!canMove)
        {
            return;
        }

        transform.Rotate(0, horizontal * Time.deltaTime * velocidadRotacion, 0);
        transform.Translate(0, 0, vertical * Time.deltaTime * velocidadMovimiento);
    }
}


