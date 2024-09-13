using System;
using UnityEngine;

public class ExpectationState : PlayerState
{
  public override void Initialize()
  {
    StateType = PlayerStateType.EXPECTATION;
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
