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
        pauseMovement = false;
    }
    

    override public void Strategy()
    {
        if (pauseMovement) { return;  }
        //i//f (playerVcinCo == null){ playerVcinCo = StartCoroutine(InPlayerVicinity()); }
        if (collidingWithPlayer)
        {
            //melee attack
            Attack newAttack = meleeAttacks[Random.Range(0, meleeAttacks.Count)];
            nextAttack.Clear();
            nextAttack.Add(newAttack);
            pathfinderRef.SetNewNavigation(newAttack);
            return;
        }
        //else if (Mathf.Abs(Vector3.Distance(transform.position, playerRef.transform.position)) < distanceFromPlayerToFlee)
        //{
        //    //flee
        //    pathfinderRef.SetNewNavigation(pathfindingState.flee, playerRef);
        //    return;
        //}
        //else if (shouldSpecialAttack)
        //{
        //    //specialattack
        //    Attack newAttack = specialAttacks[Random.Range(0, specialAttacks.Count)];
        //    nextAttack.Clear();
        //    nextAttack.Add(newAttack);
        //    pathfinderRef.SetNewNavigation(newAttack);
        //    return;
        //}
        //else if (timeInVicinity >= timeToAttack)
        //{
        //    //range attack
        //    Attack newAttack = rangeAttacks[Random.Range(0, rangeAttacks.Count)];
        //    nextAttack.Clear();
        //    nextAttack.Add(newAttack);
        //    pathfinderRef.SetNewNavigation(newAttack);
        //    return;
        //}
        //else
        //{
        //    //wander
        //    pathfinderRef.SetNewNavigation(pathfindingState.wander);
            
        //}
    }

    private void Attacked(float hpred)
    {
        playerAttacks++;
        if (playerAttacks >= powerUpMax) 
        { 
            //shouldSpecialAttack = true;
            playerAttacks = 0;
        }
    }

    private void MiniEnemyDeath()
    {
        currentMinis++;
        if (spawnedInMinis/4 >= currentMinis) {
            //shouldSpecialAttack = true;
            spawnedInMinis = currentMinis;
            playerAttacks = 0;
        }
    }

    //IEnumerator InPlayerVicinity()
    //{
    //    //while (Mathf.Abs(Vector3.Distance(transform.position, playerRef.transform.position)) < distanceFromPlayerToFlee * 2)
    //    //{
    //    //    timeInVicinity ++;
    //    //    yield return new WaitForSeconds(1f);
    //    //}
    //    timeInVicinity = 0;
    //    playerVcinCo = null;
    //}
}
