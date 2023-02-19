using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class XcomEntityIdleState : XcomEntityState
{
    public override void OnEnter()
    {
        Debug.Log("Entered Idle State!");
    }

    public override void Update(Action action)
    {
        _actionQueue.Enqueue(action);

        Action currentAction = _actionQueue.Dequeue();
        CheckStateChange(currentAction);
    }

    public override void CheckStateChange(Action action)
    {
        
    }

    public override void OnExit()
    {
        Notify(EntityState.Idle);
    }
}
