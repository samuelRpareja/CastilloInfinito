using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExpSystem : MonoBehaviour
{

    [Header("nivel y XP")]
    public float level = 1f;
    public float currentXP = 0f;
    public float xpToNextLevel = 100f;

    [Header("puntos de mejora")]
    public int upgradePoints = 0;

    public delegate void OnLevelUp(int newLevel, int upgradePoints);
    public event OnLevelUp onLevelUp;


    public void gainXP(int amount)
    {
        currentXP += amount;

        // Subir de nivel si pasa el umbral
        if (currentXP >= xpToNextLevel)
        {
            currentXP = currentXP - xpToNextLevel;
            levelUp();
        }
    }

    private void levelUp()
    {
        level++;
        upgradePoints += 1;
        xpToNextLevel *= 1.2f;

        if (onLevelUp != null)
            onLevelUp((int)level, upgradePoints);
    }

    public void SpendPoint()
    {
        if (upgradePoints > 0)
            upgradePoints--;
    }


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
