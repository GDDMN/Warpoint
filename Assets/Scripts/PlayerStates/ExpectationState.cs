using UnityEngine;

public class ExpectationState : PlayerState
{
  public override void Initialize()
  {
    StateType = PlayerStateType.EXPECTATION;
  }

  public override void Enter(ActorComponent actorComponent, CinemachineData cinemachineData,
                             CharacterController characterController, GameObject mainCamera)
  {
  }

  public override void Update(bool isAiming, bool isShooting, bool isSprint, 
                            bool isCrouch, bool isReloading, Vector2 moveDirection, 
                            bool isAnalogMovement, bool isJump)
  {
  }

  public override void LateUpdate(Vector2 look)
  {
  }

  public override void Exit()
  {
  }
}
