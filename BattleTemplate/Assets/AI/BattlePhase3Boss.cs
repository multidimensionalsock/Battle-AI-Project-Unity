using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattlePhase3Boss : BattlePhaseTemplate
{
    override public void MovementStrategy()
    {
        if (pauseMovement) { return; }

        if (m_playerRigidBody == null) { m_playerRigidBody = playerRef.GetComponent<Rigidbody>(); }
        float distanceFromPlayer = Vector3.Distance(playerRef.transform.position, transform.position);
        float degrees = Mathf.Atan2(playerRef.transform.position.y - transform.position.y, playerRef.transform.position.x - transform.position.x) * Mathf.Rad2Deg;
        float directionOfPlayer = Mathf.Atan2(0 - m_playerRigidBody.velocity.y, 0 - m_playerRigidBody.velocity.x) * Mathf.Rad2Deg;

        //check if player is moving in your rought direction (within 15 degrees)

        //if (distanceFromPlayer < distanceFromPlayerToFlee)
        //{
        //    if (directionOfPlayer > degrees - 15 || directionOfPlayer < degrees + 15)
        //    {
        //        pathfinderRef.SetNewNavigation(pathfindingState.evade, playerRef);
        //    }
        //    else
        //    {
        //        pathfinderRef.SetNewNavigation(pathfindingState.flee, playerRef);
        //    }
    }


    override public void AttackStrategy()
    {

    }
}

