using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    private CharacterController characterController;
    public Player player;    

    void Start()
    {
        characterController = GetComponent<CharacterController>();        
    }

    void Update()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        Vector3 moveDirection = new Vector3(horizontalInput, 0.0f, verticalInput);
        moveDirection = transform.TransformDirection(moveDirection);

        characterController.SimpleMove(moveDirection * player.speed);
    }
}
