using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract record Action()
{
    public record Walk(Vector2Int nextPos) : Action;
    public record Run(Vector2Int nextPos) : Action;
    public record Attack(IXcomCharacterController entity) : Action;
    public record EndTurn() : Action;
}

public abstract class XcomEntityState
{
    protected XcomEntityStateMachine _root;
    protected Queue<Action> _actionQueue;

    public XcomEntityState()
    {
          _actionQueue = new Queue<Action>();
    }

    public abstract void OnEnter();
    public abstract void Update(Action action);
    public abstract void OnExit();
    public abstract void CheckStateChange(Action action);

    protected void SwitchState(EntityState state)
    {
        OnExit();
        _root.currentState = _root.factory.GetState(state);
    }

    protected void Notify(EntityState state)
    {
        if (_root.hook.IsSome())
            _root.hook.Unwrap()(state);
    }

    protected void NotifyIn(EntityState state, float delay)
    {
        if (_root.hook.IsNone()) return;

        IEnumerator _delay()
        {
            yield return new WaitForSeconds(delay);
            _root.hook.Unwrap()(state);
        }

        //get a hold of the monobehavior at the top of the hierachy to call
        //the coroutine
        _root.controller.EnterCoroutine(_delay());
    }
    
}
