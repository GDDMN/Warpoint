using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class DeadState : PlayerState
{
  public override void Initialize()
  {
    StateType = PlayerStateType.DEAD;
  }

  public override void Enter(ActorComponent actorComponent, CinemachineData cinemachineData,
                             CharacterController characterController, StarterAssets.StarterAssetsInputs inputs,
                             GameObject mainCamera, PlayerInput playerInput)
  {
  }

  public override void Update()
  {
  }

  public override void LateUpdate()
  {
  }

  public override void Exit()
  {
  }
}