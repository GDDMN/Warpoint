using UnityEngine;

public class AliveState : PlayerState, IValidatorsSetter
{
  private float _cinemachineTargetYaw;
  private float _cinemachineTargetPitch;
  private const float _threshold = 0.01f;

  private bool _aiming;
  private bool _shooting;
  private bool _sprinting;
  private bool _crouch;
  private bool _reloading;
  private Vector2 _moveDirection;
  private bool _analogMovement;
  private bool _jump;
  private Vector3 _shootingPos;

  public override void Initialize(ActorComponent actorComponent)
  {
    ActorComponent = actorComponent;
    StateType = PlayerStateType.ALIVE;
  }

  public override void Enter(CharacterController characterController, GameObject mainCamera)
  {
    CharacterController = characterController;
    MainCamera = mainCamera;
    ActorComponent.Health = 100;

    ActorComponent.ActorValidators.IsAlive = true;
    //ActorComponent.Animator.enabled = true;
  }

  public void SetValidatorsValue(bool isAiming, bool isShooting, bool isSprint, bool isCrouch, bool isReloading, Vector2 moveDirection, bool isAnalogMovement, bool isJump, Vector2 shootingPos)
  {
    _aiming = isAiming;
    _shooting = isShooting;
    _sprinting = isSprint;
    _crouch = isCrouch;
    _reloading = isReloading;

    _moveDirection = moveDirection;
    _shootingPos = shootingPos;
    _analogMovement = isAnalogMovement;
    _jump = isJump;
  }

  public override void Update()
  {
    //Внимательно смотрим за последовательностью команд
    ActorComponent.ActorValidators.SetActorStateValidators(_aiming, _shooting , _sprinting, _crouch, 
                                          _reloading, _moveDirection, _analogMovement, _shootingPos);

    ActorComponent.JumpAndGravity(_jump);
    ActorComponent.GroundedCheck();
    ActorComponent.Move(CharacterController, MainCamera);

    ActorComponent.Reloading();
    ActorComponent.Aiming();
    ActorComponent.Shooting();
    ActorComponent.Cruch();
    ActorComponent.LegsMotionValidator();
  }

  private void SetActorValidators()
  {
  }

  public override void Exit()
  {
  }
}

public interface IValidatorsSetter
{
  void SetValidatorsValue(bool isAiming, bool isShooting, bool isSprint, 
                          bool isCrouch, bool isReloading, Vector2 moveDirection, 
                          bool isAnalogMovement, bool isJump, Vector2 shootingPos);
}
