using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattlePhaseTemplate : MonoBehaviour
{
    protected List<Attack> attacks;
    [SerializeField] protected Attacks attackData;
    protected GameObject playerRef;
    protected Pathfinding pathfinderRef;
    protected Rigidbody m_playerRigidBody;
    [SerializeField] protected float distanceFromPlayerToFlee;
    protected bool pauseMovement = false; //pause movement behaviour algorithm (used if performing attack that requires them stay still)
    public void Enable(GameObject playerreference, int battlePhase)
    {
        
        playerRef = playerreference;
        
        attacks = new List<Attack>();
        //LoadAttacks(battlePhase);
        
        m_playerRigidBody = playerRef.GetComponent<Rigidbody>();
        
        pathfinderRef = GetComponent<Pathfinding>();
        
        pathfinderRef.SetDistanceToFlee(distanceFromPlayerToFlee);
    }

    virtual public void MovementStrategy()
    {

    }

    virtual public void AttackStrategy()
    {

    }

    virtual public void LoadAttacks(int stage)
    {
        for (int i = 0; i < attackData.attackDetails.Length; i++)
        {
            if (attackData.attackDetails[i].attackStage <= stage)
            {
                attacks.Add(attackData.attackDetails[i]);
            }
        }
    }

    public void SetPlayerReference(GameObject playerreference)
    {
        playerRef = playerreference;
    }

    public void SetPathfinding(Pathfinding pathfinder)
    {
        pathfinderRef = pathfinder;
    }
}
