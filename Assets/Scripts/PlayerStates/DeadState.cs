using UnityEngine;

public class DeadState : PlayerState
{
  public override void Initialize(ActorComponent actorComponent)
  {
    ActorComponent = actorComponent;
    StateType = PlayerStateType.DEAD;
  }

  public override void Enter(CharacterController characterController, GameObject mainCamera)
  {
    ActorComponent.ActorValidators.IsAlive = false;
  }

  private void SetDeathAnimation()
  {
    ActorComponent.Animator.SetBool("Alive", false);
  }

  public override void Update()
  {
  }

  public override void Exit()
  {
  }
}