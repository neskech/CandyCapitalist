using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EntityState
{
    Idle,
    Walking,
    Running,
    Attacking,
    Dead,
    NUM_STATES
}

public class XcomEntityStateFactory
{
    XcomEntityState[] _states;
    public XcomEntityStateFactory(XcomEntityStateMachine sm)
    {
        _states = new XcomEntityState[(int)EntityState.NUM_STATES];

         _states[0] = new XcomEntityIdleState(sm);
         _states[1] = new XcomEntityWalkState(sm);
        // _states[2] = new 
        // _states[3] = new 
        // _states[4] = new 
    }

    public XcomEntityState GetState(EntityState state)
    {
        return _states[(int)state];
    }
}
