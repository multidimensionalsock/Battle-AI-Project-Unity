using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class MainBattlePhase : BattlePhaseTemplate
{
    bool fleeMode;
    Coroutine modeSwitchCoroutine;
    Coroutine InPlayerRange;
    [SerializeField] float timeToSwitchModes;
    [SerializeField] protected float distanceFromPlayerToSpawn; //distane to spawn enemies (further)
    [SerializeField] protected float distanceFromPlayerToAttack; //distance to flee/attack (closer)
    [SerializeField] protected float TimeInDistanceToAttack; //how long the player has to be in close distance to swap to attack
    [SerializeField] protected float TimeBetweenAttacks;
    [SerializeField] protected float TimeBetweenSpecialAttacks;
    bool ableToAttack;
    bool ableToSpecialAttack;

    // Start is called before the first frame update
    void Start()
    {
        fleeMode = true;
        modeSwitchCoroutine = StartCoroutine(SwitchModes());
        TimeInDistanceToAttack = TimeInDistanceToAttack * 50;
        StartCoroutine(AttackCooldown());
    }



    public override void Strategy()
    {
        if (fleeMode)
        {
            FleeStrategy();
        }
        if (!fleeMode)
        {
            AttackStategy();
        }
    }

    void FleeStrategy()
    {
        float distance = DistanceFromPlayer();
        if (distance < distanceFromPlayerToSpawn)
        {
            //distance attack if attack is allowed
            if (InPlayerRange == null)
            {
                InPlayerRange = StartCoroutine(TimeInPlayersRange());
                //spawn mini enemies if time not run out 
            }
            if (distance < distanceFromPlayerToAttack)
            {
                //flee player
                pathfinderRef.SetNewNavigation(pathfindingState.flee, playerRef);
            }
            else
            {
                //wander 
                pathfinderRef.SetNewNavigation(pathfindingState.wander);
            }
        }
        else
        {
            if (InPlayerRange != null)
            {
                StopCoroutine(InPlayerRange);
                InPlayerRange = null;
               
            }
            //wander
            pathfinderRef.SetNewNavigation(pathfindingState.wander);
        }
    }

    void AttackStategy()
    {

    }

    float DistanceFromPlayer()
    {
        return Mathf.Abs(Vector3.Distance(playerRef.transform.position, transform.position));
    }

    IEnumerator AttackCooldown()
    {
        ableToAttack = false;
        yield return new WaitForSeconds(TimeBetweenAttacks);
        ableToAttack = true;
    }

    IEnumerator SpecialAttackCooldown()
    {
        ableToSpecialAttack = false;
        yield return new WaitForSeconds(TimeBetweenSpecialAttacks);
        ableToSpecialAttack = true;
    }

    IEnumerator TimeInPlayersRange()
    {
        int loops = 0;
        while (DistanceFromPlayer() < distanceFromPlayerToSpawn)
        {
            //if in range for x seconds
            if (loops >= TimeInDistanceToAttack * 50 * Time.fixedDeltaTime)
            {
                StopCoroutine(modeSwitchCoroutine);
                fleeMode = false;
                modeSwitchCoroutine = StartCoroutine(SwitchModes());
                yield break;
            }

            loops++;
            yield return new WaitForFixedUpdate();

        }

    }

    IEnumerator SwitchModes()
    {
        yield return new WaitForSeconds(timeToSwitchModes);
        fleeMode = !fleeMode;
        modeSwitchCoroutine = StartCoroutine (SwitchModes());
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("colliding");
        if (collision.gameObject.tag != "Player") return; 
        if (ableToAttack) 
        {
            
            //get random mellee attack 
            Attack temp = PickRandomAttack(meleeAttacks);
            if (pauseMovement != true)
            {
                StartCoroutine(MovementPause(temp.freezeTime));
            }
            Debug.Log(temp.ToString());
            StartAttack(PickRandomAttack(meleeAttacks));
            AttackCooldown();
        }
    }

    IEnumerator MovementPause(float time)
    {
        pauseMovement = true; 
        yield return new WaitForSeconds(time);
        pauseMovement = false;

    }


}
