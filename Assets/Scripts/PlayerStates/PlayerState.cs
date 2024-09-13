public abstract class PlayerState
{
  public PlayerStateType StateType;

  public abstract void Initialize();

  public abstract void Enter(ActorComponent actorComponent, CinemachineData cinemachineData);

  public abstract void Update();

  public abstract void Exit();
}