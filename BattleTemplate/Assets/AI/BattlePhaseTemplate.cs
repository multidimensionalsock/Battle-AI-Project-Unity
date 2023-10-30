using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattlePhaseTemplate : MonoBehaviour
{
    protected List<Attack> meleeAttacks;
    protected List<Attack> rangeAttacks;
    protected List<Attack> specialAttacks;
    [SerializeField] protected Attacks attackData;
    protected GameObject playerRef;
    protected Pathfinding pathfinderRef;
    protected Rigidbody m_playerRigidBody;
    [SerializeField] protected float distanceFromPlayerToFlee;
    protected bool pauseMovement = false; //pause movement behaviour algorithm (used if performing attack that requires them stay still)
    public void Enable(GameObject playerreference)
    {
        playerRef = playerreference;
        //LoadAttacks();
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

    virtual public void LoadAttacks()
    {
        meleeAttacks = new List<Attack>();
        rangeAttacks = new List<Attack>();
        specialAttacks = new List<Attack>();
        for (int i = 0; i < attackData.attackDetails.Length; i++)
        {
            switch (attackData.attackDetails[i].attackType)
            {
                case AttackType.melee:
                    meleeAttacks.Add(attackData.attackDetails[i]);
                    break;
                case AttackType.range:
                    rangeAttacks.Add(attackData.attackDetails[i]);
                    break;
                case AttackType.special:
                    specialAttacks.Add(attackData.attackDetails[i]);
                    break;
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
