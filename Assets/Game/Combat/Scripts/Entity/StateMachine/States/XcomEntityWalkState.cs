using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class XcomEntityWalkState : XcomEntityState
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
        if (action is Action.EndTurn)
            SwitchState(EntityState.Idle);
        //switch to running state if running... 
        //(although that case should never show up)
    }

    public override void OnExit()
    {
        Notify(EntityState.Walking);
    }

    void FigureAction(Action action)
    {
        if (action is Action.Walk (var nextPos))
        {

        }
    }

    IEnumerator StepToTile(Vector2Int nextPos)
    {
        //get the nextpos in world coordinates and gradually move towards it
        yield return null;
    }
}
