using System;

public class ExpectationState : PlayerState
{
  public override void Initialize()
  {
    StateType = PlayerStateType.EXPECTATION;
  }

  public override void Enter(ActorComponent actorComponent, CinemachineData cinemachineData)
  {
  }

  public override void Exit()
  {
  }

  public override void Update()
  {
  }
}
