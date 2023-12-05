using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattlePhase1 : BattlePhaseTemplate
{
    [SerializeField] int powerUpMax; //everytime attacked you get a power up 
    int playerAttacks = 0;
    int spawnedInMinis = 0;
    int currentMinis = 0;
    int timeInVicinity = 0;
    [SerializeField] int timeToAttack; //if around for x time then attack
    Coroutine playerVcinCo;

    private void Start()
    {
        GetComponent<BattleScript>().HPreduce += Attacked;
        MiniEnemyFinite.Death += MiniEnemyDeath;
    }

    public void Strategy()
    {
        if (playerVcinCo != null){ playerVcinCo = StartCoroutine(InPlayerVicinity()); }
        if (collidingWithPlayer)
        {
            //melee attack
            return;
        }
        else if (Mathf.Abs(Vector3.Distance(transform.position, playerRef.transform.position)) < distanceFromPlayerToFlee)
        {
            //flee
            return;
        }
        else if (shouldSpecialAttack)
        {
            //specialattack
            return;
        }
        if (timeInVicinity >= timeToAttack)
        {
            //attack
            return;
        }
        else
        {
            //flee
            
        }
    }

    private void Attacked(float hpred)
    {
        playerAttacks++;
        if (playerAttacks >= powerUpMax) 
        { 
            shouldSpecialAttack = true;
            playerAttacks = 0;
        }
    }

    private void MiniEnemyDeath()
    {
        currentMinis++;
        if (spawnedInMinis/4 >= currentMinis) {
            shouldSpecialAttack = true;
            spawnedInMinis = currentMinis;
            playerAttacks = 0;
        }
    }

    IEnumerator InPlayerVicinity()
    {
        while (Mathf.Abs(Vector3.Distance(transform.position, playerRef.transform.position)) < distanceFromPlayerToFlee * 2)
        {
            timeInVicinity ++;
            yield return new WaitForSeconds(1f);
        }
        timeInVicinity = 0;
        playerVcinCo = null;
    }
}
