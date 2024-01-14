using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MBT;
using UnityEngine.AI;
using JetBrains.Annotations;
using System.Linq;
using System;

[AddComponentMenu("CustomBehaviour")]

#region custom_behaviour
[MBTNode(name = "CustomNode/Seek")]
public class Seek : Leaf
{
    public BoolReference somePropertyRef = new BoolReference();
    //Transform playerRef;

    // These two methods are optional, override only when needed
    // public override void OnAllowInterrupt() {}

    // This is called every tick as long as node is executed
    public override NodeResult Execute()
    {
        Debug.Log("Seeking");
        if (GetComponent<CheckConditions>().triggerWithPlayer != true)
        {
            //Vector3 PlayerLocation = blackboard.get
            //pathfinderRef.SetNewNavigation(pathfindingState.seek, playerRef);
            GetComponent<Pathfinding>().SetNewNavigation(pathfindingState.seek, GetComponent<CheckConditions>().playerRef);
            return NodeResult.running;
            
        }
        GetComponent<Pathfinding>().SetNewNavigation(pathfindingState.nullptr);
        return NodeResult.success;
    }

    // These two methods are optional, override only when needed
    // public override void OnExit() {}
    // public override void OnDisallowInterrupt() {}

    // Usually there is no needed to override this method
    public override bool IsValid()
    {
        // You can do some custom validation here
        return !somePropertyRef.isInvalid;
    }
} //working

[MBTNode(name = "CustomNode/FacePlayer")] //working?
public class FacePlayer : Leaf
{
    public BoolReference somePropertyRef = new BoolReference();

    // These two methods are optional, override only when needed
    // public override void OnAllowInterrupt() {}
    // public override void OnEnter() {}

    // This is called every tick as long as node is executed
    public override NodeResult Execute()
    {
        Debug.Log("FacePlayer");
        Vector3 lookRot = GetComponent<CheckConditions>().playerRef.transform.position - transform.position;
        transform.rotation = Quaternion.LookRotation(lookRot);
        
        return NodeResult.success;
    }

    // These two methods are optional, override only when needed
    // public override void OnExit() {}
    // public override void OnDisallowInterrupt() {}

    // Usually there is no needed to override this method
    public override bool IsValid()
    {
        // You can do some custom validation here
        return !somePropertyRef.isInvalid;
    }
}

[MBTNode(name = "CustomNode/Flee")]
public class Flee : Leaf
{
    public BoolReference somePropertyRef = new BoolReference();

    // These two methods are optional, override only when needed
    // public override void OnAllowInterrupt() {}
    // public override void OnEnter() {}

    // This is called every tick as long as node is executed
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

    // These two methods are optional, override only when needed
    // public override void OnExit() {}
    // public override void OnDisallowInterrupt() {}

    // Usually there is no needed to override this method
    public override bool IsValid()
    {
        // You can do some custom validation here
        return !somePropertyRef.isInvalid;
    }
} //need testing

[MBTNode(name = "CustomNode/Get Mellee Attack")]
public class GetMelleeAttack : Leaf
{
    public BoolReference somePropertyRef = new BoolReference();
    //Transform playerRef;

    // These two methods are optional, override only when needed
    // public override void OnAllowInterrupt() {}

    // This is called every tick as long as node is executed
    public override NodeResult Execute()
    {
        CheckConditions condition = GetComponent<CheckConditions>();
        if (!condition.ableToAttack) { return NodeResult.failure; } 
        if (!condition.meleeAttacks.Any()) { return NodeResult.failure; }
        condition.nextAttack = condition.meleeAttacks[UnityEngine.Random.Range(0, condition.meleeAttacks.Count)];
        return NodeResult.success;
    }

    // These two methods are optional, override only when needed
    // public override void OnExit() {}
    // public override void OnDisallowInterrupt() {}

    // Usually there is no needed to override this method
    public override bool IsValid()
    {
        // You can do some custom validation here
        return !somePropertyRef.isInvalid;
    }
} //needs testing

[MBTNode(name = "CustomNode/Get Range Attack")]
public class GetRangeAttack : Leaf
{
    public BoolReference somePropertyRef = new BoolReference();
    //Transform playerRef;

    // These two methods are optional, override only when needed
    // public override void OnAllowInterrupt() {}

    // This is called every tick as long as node is executed
    public override NodeResult Execute()
    {
        CheckConditions condition = GetComponent<CheckConditions>();
        if (!condition.ableToAttack) { return NodeResult.failure; }
        if (!condition.rangeAttacks.Any()) { return NodeResult.failure; }
        condition.nextAttack = condition.rangeAttacks[UnityEngine.Random.Range(0, condition.rangeAttacks.Count)];
        return NodeResult.success;
    }

    // These two methods are optional, override only when needed
    // public override void OnExit() {}
    // public override void OnDisallowInterrupt() {}

    // Usually there is no needed to override this method
    public override bool IsValid()
    {
        // You can do some custom validation here
        return !somePropertyRef.isInvalid;
    }
} //needs testing

[MBTNode(name = "CustomNode/Get Special Attack")]
public class GetSpecialAttack : Leaf
{
    public BoolReference somePropertyRef = new BoolReference();
    //Transform playerRef;

    // These two methods are optional, override only when needed
    // public override void OnAllowInterrupt() {}

    // This is called every tick as long as node is executed
    public override NodeResult Execute()
    {
        CheckConditions condition = GetComponent<CheckConditions>();
        if (!condition.ableToSpecialAttack) { return NodeResult.failure; }
        if (!condition.specialAttacks.Any()) { return NodeResult.failure; }
        condition.nextAttack = condition.specialAttacks[UnityEngine.Random.Range(0, condition.specialAttacks.Count)];
        Debug.Log(condition.nextAttack);
        return NodeResult.success;
    }

    // These two methods are optional, override only when needed
    // public override void OnExit() {}
    // public override void OnDisallowInterrupt() {}

    // Usually there is no needed to override this method
    public override bool IsValid()
    {
        // You can do some custom validation here
        return !somePropertyRef.isInvalid;
    }
} //needs testing

[MBTNode(name = "CustomNode/Navigate To Attack")]
public class AttackNavigate : Leaf
{
    public BoolReference somePropertyRef = new BoolReference();
    //Transform playerRef;

    // These two methods are optional, override only when needed
    // public override void OnAllowInterrupt() {}

    // This is called every tick as long as node is executed
    public override NodeResult Execute()
    {
        Attack attack = GetComponent<CheckConditions>().nextAttack;
        //if (attack.attackType == AttackType.melee) { GetComponent<Pathfinding>().SetNewNavigation(pathfindingState.seek, GetComponent<CheckConditions>().playerRef); return NodeResult.running; }
        float distanceFromPlayer = Mathf.Abs(Vector3.Distance(GetComponent<CheckConditions>().playerRef.transform.position, transform.position));
        if (attack.attackType == AttackType.melee)
        {
            if (GetComponent<CheckConditions>().triggerWithPlayer)
            {
                GetComponent<Pathfinding>().SetNewNavigation(pathfindingState.nullptr);
                return NodeResult.success;
            }
            GetComponent<Pathfinding>().SetNewNavigation(pathfindingState.seek, GetComponent<CheckConditions>().playerRef);
            return NodeResult.running;
        }
        else if (distanceFromPlayer > attack.maxDistanceToPerform || distanceFromPlayer < attack.minDistanceToPerform)
        {
            //Vector3 PlayerLocation = blackboard.get
            //pathfinderRef.SetNewNavigation(pathfindingState.seek, playerRef);
            GetComponent<Pathfinding>().SetNewNavigation(attack);
            return NodeResult.running;

        }
        else if (distanceFromPlayer <= attack.maxDistanceToPerform || distanceFromPlayer >= attack.minDistanceToPerform)
        {
            GetComponent<Pathfinding>().SetNewNavigation(pathfindingState.nullptr);
            return NodeResult.success;
        }
        GetComponent<Pathfinding>().SetNewNavigation(pathfindingState.nullptr);
        return NodeResult.failure;
    }

    // These two methods are optional, override only when needed
    // public override void OnExit() {}
    // public override void OnDisallowInterrupt() {}

    // Usually there is no needed to override this method
    public override bool IsValid()
    {
        // You can do some custom validation here
        return !somePropertyRef.isInvalid;
    }
} //needs testing


[MBTNode(name = "CustomNode/Perform Attack")]
public class PerformAttack : Leaf 
{
    public BoolReference somePropertyRef = new BoolReference();

    // These two methods are optional, override only when needed
    // public override void OnAllowInterrupt() {}
    // public override void OnEnter() {}

    // This is called every tick as long as node is executed
    public override NodeResult Execute()
    {
        Debug.Log("performattack");
        CheckConditions conditions = GetComponent<CheckConditions>();
        Attack attack = conditions.nextAttack;
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
        //return NodeResult.failure;
    }

    // These two methods are optional, override only when needed
    // public override void OnExit() {}
    // public override void OnDisallowInterrupt() {}

    // Usually there is no needed to override this method
    public override bool IsValid()
    {
        // You can do some custom validation here
        return !somePropertyRef.isInvalid;
    }
}


[MBTNode(name = "CustomNode/Destroy Self")]
public class DestroySelf : Leaf 
{
    public BoolReference somePropertyRef = new BoolReference();

    // These two methods are optional, override only when needed
    // public override void OnAllowInterrupt() {}
    // public override void OnEnter() {}

    // This is called every tick as long as node is executed
    public override NodeResult Execute()
    {
        Destroy(gameObject);
        return NodeResult.success;
    }

    // These two methods are optional, override only when needed
    // public override void OnExit() {}
    // public override void OnDisallowInterrupt() {}

    // Usually there is no needed to override this method
    public override bool IsValid()
    {
        // You can do some custom validation here
        return !somePropertyRef.isInvalid;
    }
}

[MBTNode(name = "CustomNode/Lock Movement")]
public class LockMovement : Leaf
{
    public BoolReference somePropertyRef = new BoolReference();

    // These two methods are optional, override only when needed
    // public override void OnAllowInterrupt() {}
    // public override void OnEnter() {}

    // This is called every tick as long as node is executed
    public override NodeResult Execute()
    {
        if (GetComponent<CheckConditions>() == null) { return NodeResult.failure; }
        if (GetComponent <CheckConditions>().MovementLocked == true ) { return NodeResult.running; }
        return NodeResult.success;
    }

    // These two methods are optional, override only when needed
    // public override void OnExit() {}
    // public override void OnDisallowInterrupt() {}

    // Usually there is no needed to override this method
    public override bool IsValid()
    {
        // You can do some custom validation here
        return !somePropertyRef.isInvalid;
    }
}

#endregion

