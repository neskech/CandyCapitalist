using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class XcomEnemyController : MonoBehaviour, IXcomCharacterController
{
    XcomEntityStateMachine _stateMachine;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public List<Action> GetActions()
    {
        return null;
    }

    public void TakeTurn(List<Action> actions)
    {
        throw new NotImplementedException();
    }

    public void TakeDamage(float damage)
    {
        throw new NotImplementedException();
    }

    public bool IsTurnOver() => _stateMachine.IsTurnOver();
}
