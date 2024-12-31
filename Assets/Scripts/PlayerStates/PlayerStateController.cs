using System.Collections.Generic;

public class PlayerStateController
{
  private List<PlayerState> _allStates = new List<PlayerState>();
  private ActorComponent _actorComponent;

  public void Initialize(ActorComponent actorComponent)
  {
    _actorComponent = actorComponent;
    _actorComponent.OnDeath += (delegate { this.GetState(PlayerStateType.DEAD); });

    ExpectationState expectationState = new ExpectationState();
    expectationState.Initialize(_actorComponent);

    AliveState aliveState = new AliveState();
    aliveState.Initialize(_actorComponent);

    DeadState deadState = new DeadState();
    deadState.Initialize(_actorComponent);

    _allStates.Add(expectationState);
    _allStates.Add(aliveState);
    _allStates.Add(deadState);
  }

  public PlayerState GetState(PlayerStateType type)
  {
    return _allStates.Find(state => state.StateType == type);
  }
}
