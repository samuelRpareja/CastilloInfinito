using UnityEngine;
using System.Collections;

public class MovementTest : MonoBehaviour
{
    [Header("Movimiento")]
    public float speed = 5f;
    public float rotationSpeed = 100f;
    
    [Header("Joystick")]
    public VirtualJoystick virtualJoystick;
    
    [Header("Animaciones")]
    public Animator animator;
    
    [Header("Ataque")]
    public bool attackPressed = false;
    
    private void Start()
    {
        // Buscar el joystick si no está asignado
        if (virtualJoystick == null)
        {
            virtualJoystick = FindObjectOfType<VirtualJoystick>();
        }
        
        // Buscar el animator si no está asignado
        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }
    }
    
    void Update()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        
        // Si hay joystick, usar su input también
        if (virtualJoystick != null)
        {
            float joystickH = virtualJoystick.GetHorizontal();
            float joystickV = virtualJoystick.GetVertical();
            
            // Solo usar joystick para movimiento, no para ataque
            if (Mathf.Abs(joystickH) > 0.1f)
                horizontal = joystickH;
            if (Mathf.Abs(joystickV) > 0.1f)
                vertical = joystickV;
        }
        
        if (Mathf.Abs(horizontal) > 0.1f || Mathf.Abs(vertical) > 0.1f)
        {
          //  Debug.Log($"MOVIMIENTO: H={horizontal:F2}, V={vertical:F2}");
            
            // Rotación
            if (Mathf.Abs(horizontal) > 0.1f)
            {
                transform.Rotate(0, horizontal * rotationSpeed * Time.deltaTime, 0);
            }
            
            // Movimiento
            if (Mathf.Abs(vertical) > 0.1f)
            {
                Vector3 move = transform.forward * vertical * speed * Time.deltaTime;
                transform.position += move;
            }
        }
        
        // Animaciones usando los mismos parámetros que AnimatorDriver
        if (animator != null)
        {
            // Usar VelX y VelY como en AnimatorDriver
            animator.SetFloat("VelX", horizontal);
            animator.SetFloat("VelY", vertical);
            
            // También mantener Speed para compatibilidad
            animator.SetFloat("Speed", Mathf.Abs(horizontal) + Mathf.Abs(vertical));
        }
        
        // Detectar ataque
        bool shouldAttack = false;
        
        // Ataque con teclado (solo en editor)
        #if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetButtonDown("Fire1"))
        {
            shouldAttack = true;
        }
        #endif
        
        // Ataque con botón de Android
        if (attackPressed)
        {
            shouldAttack = true;
            attackPressed = false; // Resetear el botón
        }
        
        if (shouldAttack && animator != null)
        {
            // Usar el mismo parámetro que AnimatorDriver: "golpeo"
            if (HasParameter("golpeo", animator))
            {
                animator.SetTrigger("golpeo");
                Debug.Log("ATAQUE EJECUTADO - Usando parámetro 'golpeo'");
            }
            else if (HasParameter("Attack", animator))
            {
                animator.SetTrigger("Attack");
                Debug.Log("ATAQUE EJECUTADO - Usando parámetro 'Attack'");
            }
            else
            {
                // Si no existe ningún parámetro de ataque, solo log
                Debug.Log("ATAQUE EJECUTADO - No se encontraron parámetros de ataque en Animator");
                
                // Alternativa: usar un parámetro booleano si existe
                if (HasParameter("IsAttacking", animator))
                {
                    animator.SetBool("IsAttacking", true);
                    StartCoroutine(ResetAttackBool());
                }
            }
        }
    }
    
    // Método público para que el botón de Android llame
    public void OnAttackButtonPressed()
    {
        attackPressed = true;
        Debug.Log("BOTÓN DE ATAQUE PRESIONADO");
    }
    
    /// <summary>
    /// Verifica si un parámetro existe en el Animator
    /// </summary>
    private bool HasParameter(string paramName, Animator animator)
    {
        foreach (AnimatorControllerParameter param in animator.parameters)
        {
            if (param.name == paramName)
                return true;
        }
        return false;
    }
    
    /// <summary>
    /// Resetea el parámetro booleano de ataque después de un tiempo
    /// </summary>
    private IEnumerator ResetAttackBool()
    {
        yield return new WaitForSeconds(0.5f); // Esperar medio segundo
        if (animator != null)
        {
            animator.SetBool("IsAttacking", false);
        }
    }
}
