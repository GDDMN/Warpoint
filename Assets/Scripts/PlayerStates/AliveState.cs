using UnityEngine;
using UnityEngine.InputSystem;

public class AliveState : PlayerState
{
  private float _cinemachineTargetYaw;
  private float _cinemachineTargetPitch;
  private const float _threshold = 0.01f;

  public override void Initialize()
  {
    StateType = PlayerStateType.ALIVE;
  }

  public override void Enter(ActorComponent actorComponent, CinemachineData cinemachineData,
                             CharacterController characterController, GameObject mainCamera)
  {
    ActorComponent = actorComponent;
    CinemachineData = cinemachineData;
    CharacterController = characterController;
    MainCamera = mainCamera;

    _cinemachineTargetYaw = CinemachineData.CinemachineCameraTarget.transform.rotation.eulerAngles.y;
  }

  public override void Update(bool isAiming, bool isShooting, bool isSprint, 
                              bool isCrouch, bool isReloading, Vector2 moveDirection, 
                              bool isAnalogMovement, bool isJump)
  {
    //Внимательно смотрим за последовательностью команд
    ActorComponent.JumpAndGravity(isJump);
    ActorComponent.SetActorStateValidators(isAiming, isShooting, isSprint, isCrouch, 
                                          isReloading, moveDirection, isAnalogMovement);
    ActorComponent.GroundedCheck();
    ActorComponent.Move(CharacterController, MainCamera);

    ActorComponent.Reloading();
    ActorComponent.Aiming();
    ActorComponent.Shooting();
    ActorComponent.Cruch();
    ActorComponent.LegsMotionValidator();
  }

  public override void LateUpdate(Vector2 look)
  {
    CameraRotation(look);
  }

  private void SetActorValidators()
  {
  }

  private void CameraRotation(Vector3 look)
  {
    // if there is an input and camera position is not fixed
    if (look.sqrMagnitude >= _threshold && !CinemachineData.LockCameraPosition)
    {
      //Don't multiply mouse input by Time.deltaTime;
      //float deltaTimeMultiplier = IsCurrentDeviceMouse ? 1.0f : Time.deltaTime;

      _cinemachineTargetYaw += look.x * 1.0f * CinemachineData.Sensativity;
      _cinemachineTargetPitch += look.y * 1.0f * CinemachineData.Sensativity;
    }

    // clamp our rotations so our values are limited 360 degrees
    _cinemachineTargetYaw = ClampAngle(_cinemachineTargetYaw, float.MinValue, float.MaxValue);
    _cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, CinemachineData.BottomClamp, CinemachineData.TopClamp);

    // Cinemachine will follow this target
    CinemachineData.CinemachineCameraTarget.transform.rotation = Quaternion.Euler(_cinemachineTargetPitch + CinemachineData.CameraAngleOverride,
        _cinemachineTargetYaw, 0.0f);
  }

  private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
  {
    if (lfAngle < -360f) lfAngle += 360f;
    if (lfAngle > 360f) lfAngle -= 360f;
    return Mathf.Clamp(lfAngle, lfMin, lfMax);
  }

  public override void Exit()
  {
  }
}
