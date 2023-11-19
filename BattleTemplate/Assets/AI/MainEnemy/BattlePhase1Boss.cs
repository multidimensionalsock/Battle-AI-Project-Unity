using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;

public class BattlePhase1Boss : BattlePhaseTemplate
{

    override public void MovementStrategy()
    {
        if(pauseMovement == true) { return; }
        if (m_playerRigidBody == null) { m_playerRigidBody = playerRef.GetComponent<Rigidbody>(); }
        float distanceFromPlayer = Vector3.Distance(playerRef.transform.position, transform.position);
        float degrees = Mathf.Atan2(playerRef.transform.position.y - transform.position.y, playerRef.transform.position.x - transform.position.x) * Mathf.Rad2Deg; 
        float directionOfPlayer = Mathf.Atan2(0 - m_playerRigidBody.velocity.y, 0 - m_playerRigidBody.velocity.x) * Mathf.Rad2Deg;

        //check if player is moving in your rought direction (within 15 degrees)
        
        if (distanceFromPlayer < distanceFromPlayerToFlee)
        {
            if (directionOfPlayer > degrees - 15 || directionOfPlayer < degrees + 15)
            {
                pathfinderRef.SetNewNavigation(pathfindingState.evade, playerRef);
            }
            else
            {
                pathfinderRef.SetNewNavigation(pathfindingState.flee, playerRef);
            }
        }

        //direction to you from player
        //boss mainly flees the player
        //if player moving within a 45 degree range towards boss then evade, if not then flee if within x distance 
    }

    override public void AttackStrategy()
    {
        if (AttacksLoaded == false) { return; }
        Vector3 lookRot = playerRef.transform.position - transform.position;
        //if current attack hasnt been performed then break here
        if (collidingWithPlayer)
        {
            if (pauseMovement == true) { return; }

            //rotate to face player
            
            transform.rotation = Quaternion.LookRotation(lookRot);
            
            pauseMovement = true;
            //melee attack
            int random;
            if (meleeAttacks.Count < 1)
            {
                random = 0;
            }
            else
            {
                random = UnityEngine.Random.Range(0, meleeAttacks.Count);
            }
            playerRef.GetComponent<BattleScript>().Attack(meleeAttacks[random].attackDamage);
            pathfinderRef.CallAttackAnimation(meleeAttacks[random]);
            StartCoroutine(UnlockMovement(meleeAttacks[random].freezeTime));
        }
        //if (nextAttack.Any() == true) { return;  } //check if an attack is already loaded 
        if ((playerRef.transform.position - transform.position).magnitude < distanceFromPlayerToFlee)
        {
            //flee
            //handled by movement function above
        }
        else if (shouldSpecialAttack == true)
        {
            //select and perform a special attack


            int random = UnityEngine.Random.Range(0, specialAttacks.Count);
            pathfinderRef.SetNewNavigation(specialAttacks[random]); //attack, distance
            shouldSpecialAttack = false;
        }
        else if (shouldAttack == true)
        {
            int random;
            
            if (rangeAttacks.Count < 1)
            {
                random = 0;
            }
            else
            {
                random = UnityEngine.Random.Range(0, rangeAttacks.Count);
            }
            shouldAttack = false;
            nextAttack.Add( rangeAttacks[random]);
            pathfinderRef.SetNewNavigation(rangeAttacks[random]);
            
            //GameObject attack = GameObject.Instantiate(rangeAttacks[random].attackObject, transform.position, transform.rotation);
            //attack.GetComponent<AttackTemplate>().CreateAttack(rangeAttacks[random], gameObject.GetComponent<BattleScript>(), Quaternion.LookRotation(lookRot));
            //InitiateAttack();
        }
    }

    override public void ActivateAttack()
    {

        //conditions to activate attack
    }
    override public void ActivateSpecialAttack()
    {
        //condionts to activate speical attack 
    }
}
