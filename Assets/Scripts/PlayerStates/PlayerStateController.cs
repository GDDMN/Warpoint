using System.Collections.Generic;

public class PlayerStateController
{
  private List<PlayerState> _allStates = new List<PlayerState>();

  public void Initialize()
  {
    ExpectationState expectationState = new ExpectationState();
    expectationState.Initialize();

    AliveState aliveState = new AliveState();
    aliveState.Initialize();

    DeadState deadState = new DeadState();
    deadState.Initialize();

    _allStates.Add(expectationState);
    _allStates.Add(aliveState);
    _allStates.Add(deadState);
  }

  public PlayerState GetState(PlayerStateType type)
  {
    return _allStates.Find(state => state.StateType == type);
  }
}
