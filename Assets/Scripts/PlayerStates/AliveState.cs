using System;

public class AliveState : PlayerState
{
  public override void Initialize()
  {
    StateType = PlayerStateType.ALIVE;
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
