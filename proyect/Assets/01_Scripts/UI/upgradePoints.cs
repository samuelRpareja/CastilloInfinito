using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class upgradePoints : MonoBehaviour
{

    public Player player;  // referencia al script del jugador
    public GameObject panelUpgradePoints; // panel de upgrades    
    
    public void Start()
    {        
    }
    //funcion para aumentar el danho
    public void damageUpgrade()
    {
        if (player.upgradePoints > 0)
        {
            player.damage += 3;
            player.upgradePoints--;
            closePanel();
        }
    }


    //funcion para aumentar la vida
    public void hpUpgrade()
    {
        if (player.upgradePoints > 0)
        {
            player.maxHP += 15;
            player.currentHP = player.maxHP;
            player.upgradePoints--;
            closePanel();
        }
    }

    public void speedUpgrade()
    {
        if (player.upgradePoints > 0)
        {
            player.upgradePoints--;
            player.speed += 5f;
            closePanel();
        }
    }

    public void closePanel()
    {
        if (player.upgradePoints <= 0)
        {
            panelUpgradePoints.SetActive(false);
            Time.timeScale = 1f;
        }
    }
}
