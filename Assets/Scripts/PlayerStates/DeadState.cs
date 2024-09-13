using System;
using UnityEngine;

public class DeadState : PlayerState
{
  public override void Initialize()
  {
    StateType = PlayerStateType.DEAD;
  }

  public override void Enter(ActorComponent actorComponent, CinemachineData cinemachineData,
                             CharacterController characterController, StarterAssets.StarterAssetsInputs inputs,
                             GameObject mainCamera)
  {
  }

  public override void Exit()
  {
  }

  public override void Update()
  {
  }
}