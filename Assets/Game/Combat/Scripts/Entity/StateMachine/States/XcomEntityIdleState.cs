using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class XcomEntityIdleState : XcomEntityState
{
    public XcomEntityIdleState(XcomEntityStateMachine root) : base(root)
    {
    }

    public override void OnEnter()
    {
        Debug.Log("Entered Idle State!");
    }

    public override void Update(Action action, Transform entity)
    {
        EnqueueAction(action);

        Action currentAction = DequeueAction();
        CheckStateChange(currentAction);
    }

    public override void CheckStateChange(Action action)
    {
        /*
            TODO either make this a dummy action (i.e. walk to their current tile)
            or do what we're doing now
        */
        if (action is Action.Walk)
        {
            Debug.Assert(_root._actionQueue.Count == 0);
            EnqueueAction(action);

            SwitchState(EntityState.Walking);

        }
    }

    public override void OnExit()
    {
        Debug.Log("Exit Idle State!");
    }
}
