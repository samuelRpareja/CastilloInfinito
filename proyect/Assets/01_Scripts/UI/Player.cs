using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEditor.SearchService;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    /*[Header("nivel y XP")]
    public float level = 1f;
    public float currentXP = 0f;
    public float xpToNextLevel = 100f;*/

    //[Header("puntos de mejora")]
    public int upgradePoints = 0;

    [Header("Estadisticas base")]
    public float damage = 10f;
    public float maxHP = 100f;
    public float speed = 200f;

    public float currentHP;

    private ExpSystem xpSystem;

    [Header("control del pj")]
    //private CharacterController characterController;

    public GameObject gameOverPanel;
    public GameObject upgradesPanel;


    // Start is called before the first frame update
    void Start()
    {
        //characterController = GetComponent<CharacterController>();
        currentHP = maxHP;
        gameOverPanel.SetActive(false);
        upgradesPanel.SetActive(false);
        Time.timeScale = 1f;

        xpSystem = GetComponent<ExpSystem>();
        xpSystem.onLevelUp += HandleLevelUp;
    }

    private void HandleLevelUp(int newLevel, int upgradePoints)
    {
        upgradesPanel.SetActive(true);
        Time.timeScale = 0f;
    }

    /********************************************* FUNCION PARA GANAR XP *********************************************/
    /* public void gainXP(int amount)
     {
         currentXP += amount;

         // Subir de nivel si pasa el umbral
         if (currentXP >= xpToNextLevel)
         {
             currentXP = currentXP - xpToNextLevel;
             levelUp();
         }
     }*/
    /***************************************************************************************************************/



    /*****************************************FUNCION PARA SUBIR DE NIVEL/******************************************/
    //public void levelUp()
    //{
    //  level++;
    // upgradePoints = 1; // se le da 1 punto para subir a alguna stat
    //xpToNextLevel = xpToNextLevel * 1.2f; /*cada que suba de nivel la cantidad de XP necesaria
    //pa subir al siguiente nivel se incrementa por 1.15, la cantidad para subir al nivel 3 seria 100 * 1.2 */
    //Debug.Log("subiste al nivel " + level + " puntos de mejora disponibles: " + upgradePoints);
    //upgradesPanel.SetActive(true);
    //}

    /***************************************************************************************************************/

    public void ClosePanel()
    {
        if (xpSystem.upgradePoints <= 0)
        {
            upgradesPanel.SetActive(false);
            Time.timeScale = 1f;
        }
    }






/*public void closePanel()
    {
        if (upgradePoints <= 0)
        {
            upgradesPanel.SetActive(false);
            Time.timeScale = 1f;
        }
    }*/


    // Update is called once per frame
    void Update()
    {

        if (upgradePoints > 0)
        {
            upgradesPanel.SetActive(true);
            Time.timeScale = 0f; // pausa el juego
        }

        

        if (Input.GetKeyDown(KeyCode.T))
        {
            currentHP -= 20;
        }

        if (Input.GetKeyDown(KeyCode.F))
        {
            upgradePoints += 1;
        }

        if(upgradePoints > 0)
        {
            upgradesPanel.SetActive(true);
            Time.timeScale = 0f;
        }

        if (currentHP <=0 )
        {
            gameOverPanel.SetActive(true);
            Time.timeScale = 0f;
        }
    }




    /*float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");        
        
        // Move character
        Vector3 moveDirection = new Vector3(horizontalInput, 0.0f, verticalInput);
        moveDirection = transform.TransformDirection(moveDirection);

        characterController.SimpleMove(moveDirection * speed);*/



    //funcion para aumentar el danho
    /*public void damageUpgrade()
    {
        if (upgradePoints > 0)
        {
            damage += 3;
            upgradePoints--;
            closePanel();
        }
    }


    //funcion para aumentar la vida
    public void hpUpgrade()
    {
        if (upgradePoints > 0)
        {
            maxHP += 15;
            currentHP = maxHP;
            upgradePoints--;
            closePanel();
        }
    }

    public void speedUpgrade()
    {
        if (upgradePoints > 0)
        {
            upgradePoints--;
            speed += 5f;
            closePanel();
        }
    }*/
}
