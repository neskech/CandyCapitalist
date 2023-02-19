using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Option;
using static Option.Option;

using HookFn = System.Func<EntityState, UnitType>;

public record StateMachineConfig(EntityStats entityStats, Weapon weapon);

public class XcomEntityStateMachine
{
   //vars
   public Transform entity;
   public XcomEntityState currentState;
   public Option<HookFn> hook;
   public readonly IXcomCharacterController controller;
   public readonly XcomEntityStateFactory factory;
   public readonly StateMachineConfig config;


   public XcomEntityStateMachine(Transform entity, HookFn hook, StateMachineConfig config)
   {
       factory = new XcomEntityStateFactory();
       currentState = factory.GetState(EntityState.Idle);

       this.hook = Some<HookFn>(hook);
       this.config = config;
       this.entity = entity;
   }

   public XcomEntityStateMachine(Transform entity, StateMachineConfig config)
   {
       factory = new XcomEntityStateFactory();
       currentState = factory.GetState(EntityState.Idle);

       this.hook = None<HookFn>();
       this.config = config;
       this.entity = entity;
   }

   public void Update(Action action)
   {
        currentState.Update(action);
   }

   public void FollowPath(List<Vector2Int> path)
   {
        //just add all the actions to the action queue of a state
        //Once the state is done with all of the actions, it should
        //Notify the monobehavior at the top 
        //The monobehavior should call this function
   }

   public void SetHook(HookFn hook)
   {
        this.hook = Some<HookFn>(hook);
   }
}
