using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public record StateMachineConfig(EntityStats entityStats, Weapon weapon);
public delegate Coroutine EnterCoroutine(IEnumerator routine);

public class XcomEntityStateMachine
{
   //vars
   public Animator animator;

   public XcomEntityState currentState;

   public readonly XcomEntityStateFactory factory;

   public readonly StateMachineConfig config;

   public readonly EnterCoroutine enterCoroutine;

   public Queue<Action> _actionQueue;

   public bool isFinished;

   public XcomEntityStateMachine(StateMachineConfig config, EnterCoroutine enterCoroutine, Animator animator)
   {
       factory = new XcomEntityStateFactory(this);
       currentState = factory.GetState(EntityState.Idle);
       _actionQueue = new Queue<Action>();

       this.config = config;
       this.enterCoroutine = enterCoroutine;
       this.animator = animator;
   }

   public void EnactAction(Action action, Transform entity)
   {
        currentState.Update(action, entity);
        EndTurn(entity);
   }

   public void EnactActionPool(List<Action> actions, Transform entity)
   {
        foreach (var act in actions)
          currentState.Update(act, entity);
        EndTurn(entity);
   }

   public void Update(Transform entity)
   {
       currentState.Update(new Action.NullAction(), entity);
   }

   public void EndTurn(Transform entity)
   {
      currentState.Update(new Action.EndTurn(), entity);
   }

   public bool IsTurnOver()
   {
     //TODO no hooks
     //TODO Ishas IsFinishedTurn() which just calls this function

     //TODO hook for tryTurn and bool func for taking turn
     //TODO for this to work each turn must be followed by an EndTurn Action to force
     //TODO the calling of EndState where we set the bool to true
      return isFinished;
   }

   public bool IsTurnPlaying()
   {
      return !IsTurnOver();
   }
}
