using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void DamageFn(float x);

public abstract record Action()
{
    public record Walk(Vector2Int nextPos) : Action;
    public record Run(Vector2Int nextPos) : Action;
    public record Attack(DamageFn damageFn, Vector2Int pos) : Action;
    public record EndTurn() : Action;
    public record NullAction() : Action;
}

public abstract class XcomEntityState
{
    private bool _isFinished;
    protected XcomEntityStateMachine _root;

    public XcomEntityState(XcomEntityStateMachine root)
    {
          _root = root;
    }

    public abstract void OnEnter();
    public abstract void Update(Action action, Transform entity);
    public abstract void OnExit();
    public abstract void CheckStateChange(Action action);

    protected void SwitchState(EntityState state)
    {
        OnExit();
        _root.currentState = _root.factory.GetState(state);
        _root.currentState.OnEnter();
    }

    protected void EnqueueAction(Action action) => _root._actionQueue.Enqueue(action);
    protected Action DequeueAction() => _root._actionQueue.Dequeue();
    protected void FinishTurn() => _root.isFinished = true;
    protected void StartTurn() => _root.isFinished = false;

    public bool IsFinished() => _isFinished;
    
}
