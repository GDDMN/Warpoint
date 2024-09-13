using System.Collections.Generic;

public class PlayerStateController
{
  private List<PlayerState> _allStates = new List<PlayerState>();

  public void Initialize()
  {
    ExpectationState expectationState = new ExpectationState();
    AliveState aliveState = new AliveState();
    DeadState deadState = new DeadState();

    _allStates.Add(expectationState);
    _allStates.Add(aliveState);
    _allStates.Add(deadState);
  }

  public PlayerState GetState(PlayerStateType type)
  {
    return _allStates.Find(state => state.StateType == type);
  }
}
