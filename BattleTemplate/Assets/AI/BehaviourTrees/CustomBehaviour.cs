using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MBT;
using UnityEngine.AI;
using JetBrains.Annotations;
using System.Linq;
using System;
using Unity.VisualScripting;
using static UnityEditor.SceneView;
using UnityEngine.InputSystem.HID;
using MBTExample;

[AddComponentMenu("CustomBehaviour")]

#region custom_behaviour
[MBTNode(name = "CustomNode/Seek")]
public class Seek : Leaf
{
    public BoolReference somePropertyRef = new BoolReference();
    public override NodeResult Execute()
    {
        if (GetComponent<CheckConditions>().triggerWithPlayer != true)
        {
            GetComponent<Pathfinding>().SetNewNavigation(pathfindingState.seek, GetComponent<CheckConditions>().playerRef);
            return NodeResult.running;
        }
        GetComponent<Pathfinding>().SetNewNavigation(pathfindingState.nullptr);
        return NodeResult.success;
    }

    public override bool IsValid()
    {
        return !somePropertyRef.isInvalid;
    }
}

//[MBTNode(name = "CustomNode/FacePlayer")] 
//public class FacePlayer : Leaf
//{
//    public BoolReference somePropertyRef = new BoolReference();

//    public override NodeResult Execute()
//    {
//        Debug.Log("FacePlayer");
//        Vector3 lookRot = GetComponent<CheckConditions>().playerRef.transform.position - transform.position;
//        transform.rotation = Quaternion.LookRotation(lookRot);
        
//        return NodeResult.success;
//    }

//    public override bool IsValid()
//    {
//        return !somePropertyRef.isInvalid;
//    }
//}

[MBTNode(name = "CustomNode/Flee")]
public class Flee : Leaf
{
    public BoolReference somePropertyRef = new BoolReference();

    public override NodeResult Execute()
    {
        //check distance from player 
        Pathfinding pathfinder = GetComponent<Pathfinding>();
        CheckConditions checkConditions = GetComponent<CheckConditions>();
        pathfinder.SetDistanceToFlee(checkConditions.distanceToFlee);
        if (Mathf.Abs(Vector3.Distance(checkConditions.playerRef.transform.position, transform.position)) < checkConditions.distanceToFlee)
        {
            pathfinder.SetNewNavigation(pathfindingState.flee, checkConditions.playerRef);
            return NodeResult.running;
        }
        return NodeResult.success;
    }

    public override bool IsValid()
    {
        return !somePropertyRef.isInvalid;
    }
}

[MBTNode(name = "CustomNode/Get Mellee Attack")]
public class GetMelleeAttack : Leaf
{
    public BoolReference somePropertyRef = new BoolReference();

    public override NodeResult Execute()
    {
        CheckConditions condition = GetComponent<CheckConditions>();
        if (!condition.ableToAttack) { return NodeResult.failure; }
        if (!condition.meleeAttacks.Any()) { return NodeResult.failure; }
        condition.SetNextAttack(condition.meleeAttacks[UnityEngine.Random.Range(0, condition.meleeAttacks.Count)]);
        return NodeResult.success;
    }

    public override bool IsValid()
    {
        return !somePropertyRef.isInvalid;
    }
}

[MBTNode(name = "CustomNode/Get Range Attack")]
public class GetRangeAttack : Leaf
{
    public BoolReference somePropertyRef = new BoolReference();

    public override NodeResult Execute()
    {
        CheckConditions condition = GetComponent<CheckConditions>();
        if (!condition.ableToAttack) { return NodeResult.failure; }
        if (!condition.rangeAttacks.Any()) { return NodeResult.failure; }
        condition.SetNextAttack(condition.rangeAttacks[UnityEngine.Random.Range(0, condition.rangeAttacks.Count)]);
        return NodeResult.success;
    }

    public override bool IsValid()
    {
        return !somePropertyRef.isInvalid;
    }
} 

[MBTNode(name = "CustomNode/Get Special Attack")]
public class GetSpecialAttack : Leaf
{
    public BoolReference somePropertyRef = new BoolReference();
    public override NodeResult Execute()
    {
        CheckConditions condition = GetComponent<CheckConditions>();
        if (!condition.ableToAttack) { return NodeResult.failure; }
        if (!condition.ableToSpecialAttack) { return NodeResult.failure; }
        if (!condition.specialAttacks.Any()) { return NodeResult.failure; }
        condition.SetNextAttack(condition.specialAttacks[UnityEngine.Random.Range(0, condition.specialAttacks.Count)]);
        return NodeResult.success;
    }

    public override bool IsValid()
    {
        return !somePropertyRef.isInvalid;
    }
} 

[MBTNode(name = "CustomNode/Navigate To Attack")]
public class AttackNavigate : Leaf
{
    public BoolReference somePropertyRef = new BoolReference();
    CheckConditions conditions;
    NavMeshAgent agent;
    Attack attack;
    Pathfinding pathfinding;

    public override void OnEnter()
    {
        conditions = GetComponent<CheckConditions>();
        agent = GetComponent<NavMeshAgent>();
        attack = conditions.GetNextAttack();
        pathfinding = GetComponent<Pathfinding>();
    }

    public override NodeResult Execute()
    {
        
        //if (attack.attackType == AttackType.melee) { GetComponent<Pathfinding>().SetNewNavigation(pathfindingState.seek, GetComponent<CheckConditions>().playerRef); return NodeResult.running; }
        float distanceFromPlayer = Mathf.Abs(Vector3.Distance(conditions.playerRef.transform.position, transform.position));
        if (attack.attackType == AttackType.melee)
        {
            if (GetComponent<CheckConditions>().triggerWithPlayer)
            {
                pathfinding.SetNewNavigation(pathfindingState.nullptr);
                return NodeResult.success;
            }
            agent.SetDestination(conditions.playerRef.transform.position);
            return NodeResult.running;
        }
        else if (distanceFromPlayer > attack.maxDistanceToPerform)
        {
            agent.SetDestination(conditions.playerRef.transform.position);
            return NodeResult.running;
        }
        else if (distanceFromPlayer < attack.minDistanceToPerform)
        {
            pathfinding.SetDistanceToFlee(attack.maxDistanceToPerform);
            pathfinding.SetNewNavigation(pathfindingState.flee, conditions.playerRef);
            return NodeResult.running;

        }
        GetComponent<Pathfinding>().SetNewNavigation(pathfindingState.nullptr);
        return NodeResult.success;
        
    }

    public override bool IsValid()
    {
        return !somePropertyRef.isInvalid;
    }
} 


[MBTNode(name = "CustomNode/Perform Attack")]   
public class PerformAttack : Leaf 
{
    public BoolReference somePropertyRef = new BoolReference();

    public override NodeResult Execute()
    {
        GetComponent<Pathfinding>().SetNewNavigation(pathfindingState.nullptr);

        CheckConditions conditions = GetComponent<CheckConditions>();
        Attack attack = conditions.GetNextAttack();
        
        Vector3 look = conditions.playerRef.transform.position - transform.position;
        float angle = 180 -  Mathf.Abs(Quaternion.Dot(Quaternion.LookRotation(look), transform.rotation) * 180);
        if (angle > 5)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(look), 0.01f);
            return NodeResult.running;
        }

        conditions.CallAttackEvent(attack);

        if (GetComponent<BattleScript>().GetTP() < attack.TPDecrease) { return NodeResult.failure; }
        if (attack.attackObject == null)
        {
            if (conditions.triggerWithPlayer == true)
            {
                conditions.playerRef.GetComponent<BattleScript>().Attack(attack.attackDamage);
            }
            return NodeResult.success;
        }
        else
        {
            GameObject attackObj = Instantiate(attack.attackObject, transform.position, transform.rotation);
            Vector3 lookRot = conditions.playerRef.transform.position - transform.position;
            attackObj.GetComponent<AttackTemplate>().CreateAttack(attack, gameObject.GetComponent<BattleScript>(), Quaternion.LookRotation(lookRot));
            if (attack.attackType == AttackType.special)
            {
                GetComponent<BattleScript>().SetTP(attack.TPDecrease);
            }
            return NodeResult.success;
        }
        
    }

    public override bool IsValid()
    {
        return !somePropertyRef.isInvalid;
    }
}


[MBTNode(name = "CustomNode/Destroy Self")]
public class DestroySelf : Leaf 
{
    public BoolReference somePropertyRef = new BoolReference();

    public override NodeResult Execute()
    {
        Destroy(gameObject);
        return NodeResult.success;
    }

    public override bool IsValid()
    {
        return !somePropertyRef.isInvalid;
    }
}

[MBTNode(name = "CustomNode/Lock Movement")]
public class LockMovement : Leaf
{
    public BoolReference somePropertyRef = new BoolReference();

    public override NodeResult Execute()
    {
        if (GetComponent<CheckConditions>() == null) { return NodeResult.failure; }
        if (GetComponent<CheckConditions>().MovementLocked == true ) { return NodeResult.running; }
        return NodeResult.success;
    }

    public override bool IsValid()
    {
        return !somePropertyRef.isInvalid;
    }
}

[MBTNode(name = "CustomNode/Wait Mode")]
public class StandStill : Leaf
{
    public BoolReference somePropertyRef = new BoolReference();
    float timeToFreeze;
    CheckConditions conditions;
    [SerializeField] float maxWaitTime = 4f;
    bool stopFreeze;

    public override void OnEnter()
    {
        stopFreeze = false;
        StartCoroutine(WaitFor());
        GetComponent<Pathfinding>().SetNewNavigation(pathfindingState.nullptr);
        conditions = GetComponent<CheckConditions>();
        float distanceFromPlayer = Mathf.Abs(Vector3.Distance(conditions.playerRef.transform.position, transform.position));
        float playerSpeed = conditions.playerRef.GetComponent<Rigidbody>().velocity.magnitude;
        timeToFreeze = (distanceFromPlayer / playerSpeed) - 1;
        if (playerSpeed == 0)
        {
            timeToFreeze = distanceFromPlayer;
        }
        if (timeToFreeze > maxWaitTime) { timeToFreeze = maxWaitTime; }
        conditions.WaitModeEventCaller(true);
    }

    public override NodeResult Execute()
    {
        if (stopFreeze) { return NodeResult.success; }
        if (conditions.triggerWithPlayer) { conditions.WaitModeEventCaller(false); return NodeResult.success; }

        Vector3 look = conditions.playerRef.transform.position - transform.position;
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(look), 0.01f);
        return NodeResult.running;
    }

    IEnumerator WaitFor()
    {
        yield return new WaitForSeconds(timeToFreeze);
        stopFreeze = true;
    }

    public override bool IsValid()
    {
        return !somePropertyRef.isInvalid;
    }
}

[MBTNode(name = "CustomNode/Spawn Helpers")]
public class SpawnHelpers : Leaf
{
    public BoolReference somePropertyRef = new BoolReference();
    float timeToFreeze;
    CheckConditions conditions;
    [SerializeField] GameObject miniEnemys;
    [SerializeField] int MinNoToSpawn;
    [SerializeField] int MaxNoToSpawn;
    [SerializeField] int maxEnemiesInScene;

    public override NodeResult Execute()
    {
        if (miniEnemys == null) { return NodeResult.failure; }

        conditions = GetComponent<CheckConditions>();
        int numMiniEnemies = conditions.GetNumberMiniEnemies(); 
        if (numMiniEnemies >= MaxNoToSpawn) { return NodeResult.failure; }

        int noToSpawn = UnityEngine.Random.Range(MinNoToSpawn, MaxNoToSpawn);
        if (noToSpawn > MaxNoToSpawn - numMiniEnemies)
            noToSpawn = MaxNoToSpawn - numMiniEnemies;

        for (int i = 0; i < noToSpawn; i++)
        {
            GameObject temp = Instantiate(miniEnemys, RandomPosition(), Quaternion.Euler(0f, (360f /noToSpawn * i), 0f));
            temp.GetComponent<MiniEnemyFinite>().OnInstantiation(conditions.playerRef, gameObject);
            temp.gameObject.SetActive(true);
        }

        conditions.AddMiniEnemys(noToSpawn);

        return NodeResult.running;
    }

    private Vector3 RandomPosition()
    {
        Vector3 pos;
        Vector2 pointInCircle = UnityEngine.Random.insideUnitCircle * 10f;
        pos = new Vector3(pointInCircle.x, -1.51f, pointInCircle.y);
        return pos;
    }

    public override bool IsValid()
    {
        return !somePropertyRef.isInvalid;
    }
}

#endregion

