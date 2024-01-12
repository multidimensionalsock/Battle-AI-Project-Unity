using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MBT;
using UnityEngine.AI;

[AddComponentMenu("CustomBehaviour")]
[MBTNode(name = "CustomNode/Seek")]
public class Seek : Leaf
{
    public BoolReference somePropertyRef = new BoolReference();
    public Blackboard blackboard;
    //Transform playerRef;

    // These two methods are optional, override only when needed
    // public override void OnAllowInterrupt() {}
    public override void OnEnter() 
    {
        blackboard = GetComponent<Blackboard>();
        //playerRef = blackboard.GetVariable<TransformVariable>("PlayerT").transform;
       // Debug.Log(playerRef.gameObject.name);
    }

    // This is called every tick as long as node is executed
    public override NodeResult Execute()
    {
        //Debug.Log(playerRef.gameObject);
        //code I want executing
        //check if not colliding via pathfidning 
        if (GetComponent<CheckConditions>().collidingWithPlayer != true)
        {
            Debug.Log("in wrong");
            //Vector3 PlayerLocation = blackboard.get
            //pathfinderRef.SetNewNavigation(pathfindingState.seek, playerRef);
            GetComponent<Pathfinding>().SetNewNavigation(pathfindingState.seek, GetComponent<CheckConditions>().playerRef);
            
            //if what i want to happen happens 
            return NodeResult.failure;
        }
        //if not
        Debug.Log("in loc");
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

//custom variables 
[AddComponentMenu("CustomVar")]
public class AgentReference : Variable<NavMeshAgent>
{
    protected override bool ValueEquals(NavMeshAgent val1, NavMeshAgent val2)
    {
        return val1 == val2;
    }
}