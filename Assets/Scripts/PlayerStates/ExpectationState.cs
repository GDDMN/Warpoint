using UnityEngine;

public class ExpectationState : PlayerState
{
  public override void Initialize()
  {
    StateType = PlayerStateType.EXPECTATION;
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
