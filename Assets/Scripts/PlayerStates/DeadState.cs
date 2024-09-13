using System;

public class DeadState : PlayerState
{
  public override void Initialize()
  {
    StateType = PlayerStateType.DEAD;
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