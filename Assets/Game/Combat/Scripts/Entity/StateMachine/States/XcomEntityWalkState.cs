using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class XcomEntityWalkState : XcomEntityState
{
    bool done = true;
    public override void OnEnter()
    {
        StartTurn();
        Debug.Log("Entered Idle State!");
    }

    public override void Update(Action action, Transform entity)
    {
        _actionQueue.Enqueue(action);

        if (!done) return;

        _root.animator.Play("Walk left");

        Action currentAction = _actionQueue.Dequeue();
        FigureAction(currentAction, entity);
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
        FinishTurn();
    }

    void FigureAction(Action action, Transform entity)
    {
        if (action is Action.Walk (var nextPos))
        {
           _root.enterCoroutine(StepToTile(nextPos, entity));
        }
    }

    //TODO while this is not done, don't pop off of the queue
    IEnumerator StepToTile(Vector2Int nextPos, Transform entity)
    {
        done = false;
        Vector2 nextPosWorld = TileMaster.FromIsometricBasis(nextPos);

        float startTime = 0;
        float currTime = 0;
        float duration = 1;

        //get the nextpos in world coordinates and gradually move towards it
        while (Vector2.Distance(entity.transform.position, nextPosWorld) >= 0.001f)
        {
            currTime += Time.deltaTime;

            //gradually move currentPos to nextpos
            float t = (currTime - startTime) / duration;
            entity.transform.position = Vector2.Lerp(entity.transform.position, nextPosWorld, t);
            yield return null;

        }

        done = true;

    }
}
