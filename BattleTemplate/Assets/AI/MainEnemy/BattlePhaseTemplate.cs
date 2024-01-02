using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

public class BattlePhaseTemplate : MonoBehaviour
{
    protected List<Attack> meleeAttacks;
    protected List<Attack> rangeAttacks;
    protected List<Attack> specialAttacks;
    [SerializeField] protected Attacks attackData;
    protected GameObject playerRef;
    protected Pathfinding pathfinderRef;
    protected BattleScript battleScript;
    
    protected bool pauseMovement = false; //pause movement behaviour algorithm (used if performing attack that requires them stay still)
    protected NavMeshAgent navmesh;
    protected List<Attack> nextAttack;
    protected bool shouldSpecialAttack = false;
    protected bool shouldAttack = false;
    protected bool collidingWithPlayer;
    protected bool AttacksLoaded = false;
    [SerializeField] protected float distanceFromPlayerToFlee;



    public void Enable(GameObject playerreference)
    {
        playerRef = playerreference;
        LoadAttacks();
        //_playerRigidBody = playerRef.GetComponent<Rigidbody>();
        pathfinderRef = GetComponent<Pathfinding>();
        pathfinderRef.SetDistanceToFlee(distanceFromPlayerToFlee);
        pathfinderRef.callAttack += StartAttack;
        navmesh = GetComponent<NavMeshAgent>();
        battleScript = GetComponent<BattleScript>();
        shouldAttack = true;
    }

    virtual public void Strategy()
    {

    }

    public void StartAttack(Attack attack)
    {
        StartCoroutine(InitiateAttack());
    }

    protected IEnumerator InitiateAttack()
    {
        if (!nextAttack.Any()) { yield break; } //if nothing in attack list then do nothing 
        //face the player
        pauseMovement = true;
        navmesh.isStopped = true;
        pathfinderRef.SetNewNavigation(pathfindingState.nullptr);
        GameObject attack = GameObject.Instantiate(nextAttack[0].attackObject, transform.position, transform.rotation);
        Vector3 lookRot = playerRef.transform.position - transform.position;
        attack.GetComponent<AttackTemplate>().CreateAttack(nextAttack[0], gameObject.GetComponent<BattleScript>(), Quaternion.LookRotation(lookRot)); //no rwef setr 
        if (nextAttack[0].attackType == AttackType.special)
        {
            battleScript.SetTP(nextAttack[0].TPDecrease);
        }
        nextAttack.Clear();
        //create the attack or and perform aniamtin
        yield return new WaitForSeconds(nextAttack[0].freezeTime);
        navmesh.isStopped = false;
        pauseMovement = false;
        
    }

    //protected IEnumerator AnimationDelay(Attack attack)
    //{
    //    //play animation
    //    yield return new WaitForSeconds(attack.associatedAnimation.length);
    //    if (collidingWithPlayer)
    //    {
    //        playerRef.GetComponent<BattleScript>().Attack(attack.attackDamage + battleScript.m_Attack);
    //    }
    //}


    virtual public void LoadAttacks()
    {
        meleeAttacks = new List<Attack>();
        rangeAttacks = new List<Attack>();
        specialAttacks = new List<Attack>();
        nextAttack = new List<Attack>();
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
        AttacksLoaded = true;
    }

    protected Attack PickRandomAttack(List<Attack> attacks)
    {
        int Index = UnityEngine.Random.Range(0, attacks.Count);
        return attacks[Index];
    }

    protected IEnumerator MovementPause(float time)
    {
        pauseMovement = true;
        yield return new WaitForSeconds(time);
        pauseMovement = false;
    }

    public void SetPlayerReference(GameObject playerreference)
    {
        playerRef = playerreference;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.GetComponent<PlayerAnimator>() != null)
        {
            collidingWithPlayer = true;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.GetComponent<PlayerAnimator>() != null)
        {
            collidingWithPlayer = false;
            pauseMovement = false;
        }
    }
}
