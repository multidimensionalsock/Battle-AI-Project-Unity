using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MBT;
using UnityEngine.AI;
using JetBrains.Annotations;

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
        Debug.Log("Seeking");
        if (GetComponent<CheckConditions>().triggerWithPlayer != true)
        {
            //Vector3 PlayerLocation = blackboard.get
            //pathfinderRef.SetNewNavigation(pathfindingState.seek, playerRef);
            GetComponent<Pathfinding>().SetNewNavigation(pathfindingState.seek, GetComponent<CheckConditions>().playerRef);
            return NodeResult.running;
            
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
}

[MBTNode(name = "CustomNode/FacePlayer")]
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

[MBTNode(name = "CustomNode/FacePlayer")]
public class Flee : Leaf
{
    public BoolReference somePropertyRef = new BoolReference();

    // These two methods are optional, override only when needed
    // public override void OnAllowInterrupt() {}
    // public override void OnEnter() {}

    // This is called every tick as long as node is executed
    public override NodeResult Execute()
    {
        Debug.Log("Flee");
        GetComponent<Pathfinding>().SetNewNavigation(pathfindingState.flee, GetComponent<CheckConditions>().playerRef);
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