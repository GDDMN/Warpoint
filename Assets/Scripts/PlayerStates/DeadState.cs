using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class DeadState : PlayerState
{
  public override void Initialize()
  {
    StateType = PlayerStateType.DEAD;
  }

  public override void Enter(ActorComponent actorComponent,
                             CharacterController characterController, GameObject mainCamera)
  {
  }

  public override void Update()
  {
  }
  
  public override void Exit()
  {
  }
}