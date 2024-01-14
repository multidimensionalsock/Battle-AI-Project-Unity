using MBT;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

#region custom_variables
[AddComponentMenu("CustomVar")]
public class AgentReference : Variable<NavMeshAgent>
{
    protected override bool ValueEquals(NavMeshAgent val1, NavMeshAgent val2)
    {
        return val1 == val2;
    }
}



#endregion

