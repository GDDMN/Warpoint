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
    int deathTypeAnimation = Random.Range(0, 5);
    ActorComponent.Animator.SetInteger("DeathAnimationValue", deathTypeAnimation);

    ActorComponent.ActorValidators.IsAlive = false;
    ActorComponent.Animator.SetTrigger("DeadTrigger");
  }

  private void SetDeathAnimation()
  {
  }

  public override void Update()
  {
  }

  public override void Exit()
  {
  }
}