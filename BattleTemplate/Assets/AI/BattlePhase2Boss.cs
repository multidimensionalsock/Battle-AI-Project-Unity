using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.RestService;
using UnityEngine;

public class BattlePhase2Boss : BattlePhaseTemplate
{
    bool attackMode = false; //stop flee behaviour 

    override public void MovementStrategy()
    {
        if(pauseMovement) { return; }

        if (m_playerRigidBody == null) { m_playerRigidBody = playerRef.GetComponent<Rigidbody>(); }
        float distanceFromPlayer = Vector3.Distance(playerRef.transform.position, transform.position);
        float degrees = Mathf.Atan2(playerRef.transform.position.y - transform.position.y, playerRef.transform.position.x - transform.position.x) * Mathf.Rad2Deg;
        float directionOfPlayer = Mathf.Atan2(0 - m_playerRigidBody.velocity.y, 0 - m_playerRigidBody.velocity.x) * Mathf.Rad2Deg;

        if (attackMode == true)
        {
            //steering related to attackmode
            //try to stay within range of player with short breaks for flee mode?
            //deciosn making about when to switch to this needs cementing 
        }
        if (attackMode == false)
        {
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
        }

        //check if player is moving in your rought direction (within 15 degrees)



        //direction to you from player
        //boss mainly flees the player
        //if player moving within a 45 degree range towards boss then evade, if not then flee if within x distance 
    }

    //void AttackMovement(Attack attackData)
    //{
    //    pauseMovement = true;
    //    float minDistance = attackData.minDistanceToPerform;
    //    float maxDistance = attackData.maxDistanceToPerform;
        
    //    float distance = Mathf.Abs(Vector3.Distance(playerRef.transform.position, transform.position));
    //    Vector3 anglefromPlayer = (playerRef.transform.position - transform.position).normalized;
    //    float timeToMove = 0;
    //    if (distance > maxDistance) 
    //    { 
    //        //move to max distance
    //        Vector3 pos = playerRef.transform.position + (anglefromPlayer * maxDistance);
    //        timeToMove = Mathf.Abs(Vector3.Distance(pos, transform.position)) / navmeshSpeed;
    //        pathfinderRef.SetNewNavigation(pathfindingState.seek, pos);
    //        //face player
    //        //spawn attack
    //    }
    //    else if (maxDistance < distance && maxDistance < minDistance)
    //    {
    //        //face the player
    //        // spawn attack forward
    //    }
    //    if (distance < minDistance)
    //    {
    //        //seek to midpoint between min and max
    //        Vector3 pos = playerRef.transform.position + (anglefromPlayer * (maxDistance - minDistance));
    //        timeToMove = Mathf.Abs(Vector3.Distance(pos, transform.position)) / navmeshSpeed;
    //        pathfinderRef.SetNewNavigation(pathfindingState.seek, pos);
    //    }
    //    //StartCoroutine(UnpauseMovement(attackData));
    //}

   
    override public void AttackStrategy()
    {
        if (nextAttack.Any()) { return; }
        float distanceFromPlayer = 0;
        //choose attack (if necessary)
        ////add attack to list
        ////choose how far from the player you need to be to attack (look at above function and rewrite here)

        if(nextAttack.Any())
        {
            pathfinderRef.SetNewNavigation(nextAttack[0], distanceFromPlayer);
            //pathfind to attack distance
        }

    }
}
