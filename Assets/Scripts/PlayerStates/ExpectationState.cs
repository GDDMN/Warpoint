using UnityEngine;

public class ExpectationState : PlayerState
{
  public override void Initialize(ActorComponent actorComponent)
  {
    ActorComponent = actorComponent;
    StateType = PlayerStateType.EXPECTATION;
  }

  public override void Enter(CharacterController characterController, GameObject mainCamera)
  {
  }

  public override void Update()
  {
  }

  public override void Exit()
  {
  }
}
