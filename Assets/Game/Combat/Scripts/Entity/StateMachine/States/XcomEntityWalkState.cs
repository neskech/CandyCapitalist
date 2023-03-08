using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class XcomEntityWalkState : XcomEntityState
{
    int walkNorth = Animator.StringToHash("Walk N");
    int walkSouth = Animator.StringToHash("Walk S");
    int walkWest = Animator.StringToHash("Walk W");
    int walkEast = Animator.StringToHash("Walk E");
    
    int idleNorth = Animator.StringToHash("Idle N");
    int idleSouth = Animator.StringToHash("Idle S");
    int idleWest = Animator.StringToHash("Idle W");
    int idleEast = Animator.StringToHash("Idle E");

    int lastPlayedWalkAnimation = -1;

    bool done = true;

    public XcomEntityWalkState(XcomEntityStateMachine root) : base(root)
    {
    }

    public override void OnEnter()
    {
        StartTurn();
        Debug.Log("Entered Idle State!");
    }

    public override void Update(Action action, Transform entity)
    {
        if (! (action is Action.NullAction))
          EnqueueAction(action);

        if (!done) return;

        Action currentAction = DequeueAction();
        CheckStateChange(currentAction);
        FigureAction(currentAction, entity);

    }

    public override void CheckStateChange(Action action)
    {
        if (action is Action.EndTurn)
        {
            SwitchState(EntityState.Idle);
        }

        //switch to running state if running... 
        //(although that case should never show up)
    }

    public override void OnExit()
    {
        Debug.Log("FREEEEEDOOMMMMM!!!");
        FinishTurn();
    }

    void FigureAction(Action action, Transform entity)
    {
        if (action is Action.Walk (var nextPos))
        {
            PlayAppropiateAnimation(nextPos, entity);

           _root.enterCoroutine(StepToTile(nextPos, entity));
        }
        else if (action is Action.EndTurn)
        {
            int aniIdleHash = (lastPlayedWalkAnimation) switch 
            {
                _ when lastPlayedWalkAnimation == walkNorth => idleNorth,
                _ when lastPlayedWalkAnimation == walkSouth => idleSouth,
                _ when lastPlayedWalkAnimation == walkEast => idleEast,
                _ when lastPlayedWalkAnimation == walkWest => idleWest,
                _ => throw new UnreachableException()
            };

            _root.animator.Play(aniIdleHash);
        }
    }

    void PlayAppropiateAnimation(Vector2Int nextPos, Transform entity)
    {
        Vector2 conv = TileMaster.ToIsometricBasis(entity.position);

        Vector2Int currPos = conv.CeilToInt();
        Vector2Int displacement = nextPos - currPos;

        if (Mathf.Abs(displacement.x) + Mathf.Abs(displacement.y) > 1)
        {
            currPos = conv.FloorToInt();
            displacement = nextPos - currPos;
        }

        Debug.Log(currPos + " " + displacement + " " + nextPos);

        int aniHash = ((displacement.x, displacement.y)) switch
        {
            (1, 0) => walkEast,
            (0, 1) => walkSouth,
            (-1, 0) => walkNorth,
            (0, -1) => walkWest,
            _ => throw new UnreachableException()
        };

        _root.animator.Play(aniHash);
        lastPlayedWalkAnimation = aniHash;
    }

    //TODO while this is not done, don't pop off of the queue
    IEnumerator StepToTile(Vector2Int nextPosition, Transform entity)
    {
        done = false;

        Vector2Int nextPos = new Vector2Int(nextPosition.x, nextPosition.y);
        Vector3 intialPos = entity.transform.position;

        const float DELTA = 0.001f;

        Vector2 nextPosWorld = TileMaster.FromIsometricBasis(nextPos)
                               + TileMaster.FromIsometricBasis(new Vector2(0.5f, 0.5f));

        float startTime = 0;
        float currTime = 0;
        float duration = 0.4f;

        //get the nextpos in world coordinates and gradually move towards it
        while (Vector2.Distance(entity.transform.position, nextPosWorld) > DELTA)
        {
            currTime += Time.deltaTime;

            //gradually move currentPos to nextpos
            float t = (currTime - startTime) / duration;
            entity.transform.position = Vector2.Lerp(intialPos, nextPosWorld, t);
            yield return null;
        }

        done = true;

    }
}
