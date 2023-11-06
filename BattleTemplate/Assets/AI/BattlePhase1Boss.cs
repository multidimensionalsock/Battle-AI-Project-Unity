using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BattlePhase1Boss : BattlePhaseTemplate
{
    override public void MovementStrategy()
    {
        if(pauseMovement) { return; }
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
        //if current attack hasnt been performed then break here
        if (collidingWithPlayer)
        {
            //melee attack
            int random = UnityEngine.Random.Range(0, meleeAttacks.Count);
            StartCoroutine(AnimationDelay(meleeAttacks[random]));
            //perform animation here

            //the below shouldnt be done until the animation has played, when it ends if still colliding then inflict damage 
            
        }
        if (nextAttack.Any() == true) { return;  } //check if an attack is already loaded 
        if ((playerRef.transform.position - transform.position).magnitude < distanceFromPlayerToFlee)
        {
            //flee
            //handled by movement function above
        }
        else if (shouldSpecialAttack == true)
        {
            //select and perform a special attack
            int random = UnityEngine.Random.Range(0, specialAttacks.Count);
            GameObject attack = GameObject.Instantiate(specialAttacks[random].attackObject, transform.position, transform.rotation);
            attack.GetComponent<AttackTemplate>().CreateAttack(specialAttacks[random], gameObject.GetComponent<BattleScript>());
            InitiateAttack();
            battleScript.SetTP(specialAttacks[random].TPDecrease);
        }
        else if (shouldAttack == true)
        {
            //select and perform an attack 
            int random = UnityEngine.Random.Range(0, rangeAttacks.Count);
            GameObject attack = GameObject.Instantiate(rangeAttacks[random].attackObject, transform.position, transform.rotation);
            attack.GetComponent<AttackTemplate>().CreateAttack(rangeAttacks[random], gameObject.GetComponent<BattleScript>());
            InitiateAttack();
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
