using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Animations;
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
    [SerializeField] protected float TimeBetweenSpecialAttacks;
    bool justAttacked = false;
    bool playerInView = false;

    // Start is called before the first frame update
    void Start()
    {
        fleeMode = false;
        //modeSwitchCoroutine = StartCoroutine(SwitchModes());
        TimeInDistanceToAttack = TimeInDistanceToAttack * 50;
        ableToAttack = true;
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
        if (pauseMovement) return;
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
        if (pauseMovement) { return; }

        if (justAttacked)
        {
            justAttacked = false;
            //turn to player
            return;
        }
        if (ableToAttack)
        {
            if (collidingWithPlayer)
            {
                //mellee attack 
                Attack temp = PickRandomAttack(meleeAttacks);
            
                StartCoroutine(MovementPause(temp.freezeTime));
                nextAttack.Add(temp);
                InitiateAttack(temp);
        
                return;
            }
            else if (ableToSpecialAttack)
            {
                //special attack 
                Attack temp = PickRandomAttack(specialAttacks);
                nextAttack.Add(temp);
                pathfinderRef.SetNewNavigation(temp);
            }
            else if (DistanceFromPlayer() <= distanceFromPlayerToAttack)
            {
                //attack
                Attack temp = PickRandomAttack(rangeAttacks);
                nextAttack.Add(temp);
                pathfinderRef.SetNewNavigation(temp);
            }
            else if (playerInView)
            {
                //seek
                pathfinderRef.SetNewNavigation(pathfindingState.seek, playerRef);
            }
            else
            {
                StartCoroutine(MovementPause(0.5f)); //this needs coding still, see below 
                //need to add look around animation to this
            }
        }
        else if (DistanceFromPlayer() <= distanceFromPlayerToFlee)
        {
            //flee
            pathfinderRef.SetNewNavigation(pathfindingState.flee, playerRef);
        }
        else
        {
            //wander
            pathfinderRef.SetNewNavigation(pathfindingState.wander);
        }

    }

   

    float DistanceFromPlayer()
    {
        return Mathf.Abs(Vector3.Distance(playerRef.transform.position, transform.position));
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
        Debug.Log(fleeMode);
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("colliding");
        if (collision.gameObject.tag != "Player") return;
        collidingWithPlayer = true;
        
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.tag != "Player") return;
        collidingWithPlayer = false;
    }


    

    void Attacked(float damage)
    {
        justAttacked = true;
    }


}
