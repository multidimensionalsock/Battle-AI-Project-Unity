using MBT;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//all of these have yet to be coded

#region distanceRelatedConditions
[MBTNode(name = "Conditions/Player in Flee Range?")]
public class InFleeRange: Condition
{
    public Abort abort;
    public BoolReference somePropertyRef = new BoolReference(VarRefMode.DisableConstant);

    public override bool Check()
    {
        CheckConditions data = GetComponent<CheckConditions>();
        return (Mathf.Abs(Vector3.Distance(data.playerRef.transform.position, transform.position)) < data.distanceToFlee);
    }

    public override void OnAllowInterrupt()
    {
        // Do not listen any changes if abort is disabled
        if (abort != Abort.None)
        {
            // This method cache current tree state used later by abort system
            ObtainTreeSnapshot();
            // If somePropertyRef is constant, then null exception will be thrown.
            // Use somePropertyRef.isConstant in case you need constant enabled.
            // Constant variable is disabled here, so it is safe to do this.
            //somePropertyRef.GetVariable().AddListener(OnVariableChange);
        }
    }

    public override void OnDisallowInterrupt()
    {
        if (abort != Abort.None)
        {
            // Remove listener
            somePropertyRef.GetVariable().RemoveListener(OnVariableChange);
        }
    }

    private void OnVariableChange(bool oldValue, bool newValue)
    {
        // Reevaluate Check() and abort tree when needed
        EvaluateConditionAndTryAbort(abort);
    }
}

[MBTNode(name = "Conditions/Colliding with Player?")]
public class CollidingPlayer : Condition
{
    public Abort abort;
    public BoolReference somePropertyRef = new BoolReference(VarRefMode.DisableConstant);

    public override bool Check()
    {
        if (GetComponent<CheckConditions>().collidingWithPlayer)
        {
            somePropertyRef.Value = false;
            return false;
        }
        somePropertyRef.Value = true;
        return true;
    }

    public override void OnAllowInterrupt()
    {
        // Do not listen any changes if abort is disabled
        if (abort != Abort.None)
        {
            // This method cache current tree state used later by abort system
            ObtainTreeSnapshot();
            // If somePropertyRef is constant, then null exception will be thrown.
            // Use somePropertyRef.isConstant in case you need constant enabled.
            // Constant variable is disabled here, so it is safe to do this.
            //somePropertyRef.GetVariable().AddListener(OnVariableChange);
        }
    }

    public override void OnDisallowInterrupt()
    {
        if (abort != Abort.None)
        {
            // Remove listener
            somePropertyRef.GetVariable().RemoveListener(OnVariableChange);
        }
    }

    private void OnVariableChange(bool oldValue, bool newValue)
    {
        // Reevaluate Check() and abort tree when needed
        EvaluateConditionAndTryAbort(abort);
    }
}

[MBTNode(name = "Conditions/Not in player Trigger Zone?")]
public class TriggeringPlayer : Condition
{
    public Abort abort;
    public BoolReference somePropertyRef = new BoolReference(VarRefMode.DisableConstant);

    public override bool Check()
    {
        if (GetComponent<CheckConditions>().triggerWithPlayer)
        {
            return false;
        }
        return true;
    }

    public override void OnAllowInterrupt()
    {
        // Do not listen any changes if abort is disabled
        if (abort != Abort.None)
        {
            // This method cache current tree state used later by abort system
            ObtainTreeSnapshot();
            // If somePropertyRef is constant, then null exception will be thrown.
            // Use somePropertyRef.isConstant in case you need constant enabled.
            // Constant variable is disabled here, so it is safe to do this.
            //somePropertyRef.GetVariable().AddListener(OnVariableChange);
        }
    }

    public override void OnDisallowInterrupt()
    {
        if (abort != Abort.None)
        {
            // Remove listener
            somePropertyRef.GetVariable().RemoveListener(OnVariableChange);
        }
    }

    private void OnVariableChange(bool oldValue, bool newValue)
    {
        // Reevaluate Check() and abort tree when needed
        EvaluateConditionAndTryAbort(abort);
    }
}

#endregion