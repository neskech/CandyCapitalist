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
}

public abstract class XcomEntityState
{
    protected bool _isFinished;
    protected XcomEntityStateMachine _root;
    protected Queue<Action> _actionQueue;

    public XcomEntityState()
    {
          _actionQueue = new Queue<Action>();
    }

    public abstract void OnEnter();
    public abstract void Update(Action action, Transform entity);
    public abstract void OnExit();
    public abstract void CheckStateChange(Action action);

    protected void SwitchState(EntityState state)
    {
        OnExit();
        _root.currentState = _root.factory.GetState(state);
    }

    protected void FinishTurn() => _isFinished = true;
    protected void StartTurn() => _isFinished = false;

    public bool IsFinished() => _isFinished;
    
}
