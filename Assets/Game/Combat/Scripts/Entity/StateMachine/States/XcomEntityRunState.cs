using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class XcomEntityRunState : XcomEntityState
{
    public XcomEntityRunState(XcomEntityStateMachine root) : base(root)
    {
    }

    public override void OnEnter()
    {
        StartTurn();
        Debug.Log("Entered Idle State!");
    }

    public override void Update(Action action, Transform entity)
    {
        EnqueueAction(action);

        Action currentAction = DequeueAction();
        FigureAction(currentAction, entity);
        CheckStateChange(currentAction);
    }

    public override void CheckStateChange(Action action)
    {
        if (action is Action.EndTurn)
             SwitchState(EntityState.Idle);
        //switch to walk state if walking... 
        //(although that case should never show up)
    }

    public override void OnExit()
    {
        FinishTurn();
    }

    void FigureAction(Action action, Transform entity)
    {
        if (action is Action.Run (var nextPos))
        {

        }
    }

    IEnumerator StepToTile(Vector2Int nextPos, Transform entity)
    {
        //get the nextpos in world coordinates and gradually move towards it
        yield return null;
    }
}
