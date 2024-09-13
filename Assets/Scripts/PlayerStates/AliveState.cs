using System;
using UnityEngine;

public class AliveState : PlayerState
{
  public override void Initialize()
  {
    StateType = PlayerStateType.ALIVE;
  }

  public override void Enter(ActorComponent actorComponent, CinemachineData cinemachineData,
                             CharacterController characterController, StarterAssets.StarterAssetsInputs inputs,
                             GameObject mainCamera)
  {
    ActorComponent = actorComponent;
    CinemachineData = cinemachineData;
    CharacterController = characterController;
    Input = inputs;
    MainCamera = mainCamera;
  }

  public override void Update()
  {
    ActorComponent.JumpAndGravity(Input);
    ActorComponent.GroundedCheck();
    ActorComponent.Move(Input, CinemachineData, CharacterController, MainCamera);
    ActorComponent.Aiming(Input, CinemachineData);
    ActorComponent.Cruch(Input);

  }

  public override void Exit()
  {
  }
}
