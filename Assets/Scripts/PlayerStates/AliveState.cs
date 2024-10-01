using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class AliveState : PlayerState
{
  private float _cinemachineTargetYaw;
  private float _cinemachineTargetPitch;
  private const float _threshold = 0.01f;

  private bool IsCurrentDeviceMouse
  {
    get
    {
#if ENABLE_INPUT_SYSTEM && STARTER_ASSETS_PACKAGES_CHECKED
      return PlayerInput.currentControlScheme == "KeyboardMouse";
#else
				return false;
#endif
    }
  }

  public override void Initialize()
  {
    StateType = PlayerStateType.ALIVE;
  }

  public override void Enter(ActorComponent actorComponent, CinemachineData cinemachineData,
                             CharacterController characterController, StarterAssets.StarterAssetsInputs inputs,
                             GameObject mainCamera, PlayerInput playerInput)
  {
    ActorComponent = actorComponent;
    CinemachineData = cinemachineData;
    CharacterController = characterController;
    Input = inputs;
    MainCamera = mainCamera;
    PlayerInput = playerInput;

    _cinemachineTargetYaw = CinemachineData.CinemachineCameraTarget.transform.rotation.eulerAngles.y;
  }

  public override void Update()
  {
    ActorComponent.JumpAndGravity(Input);
    ActorComponent.GroundedCheck();
    ActorComponent.Move(Input, CinemachineData, CharacterController, MainCamera);
    ActorComponent.Aiming(Input, CinemachineData);
    ActorComponent.Shooting(Input);
    ActorComponent.Cruch(Input);
  }

  public override void LateUpdate()
  {
    CameraRotation();
  }

  private void CameraRotation()
  {
    // if there is an input and camera position is not fixed
    if (Input.look.sqrMagnitude >= _threshold && !CinemachineData.LockCameraPosition)
    {
      //Don't multiply mouse input by Time.deltaTime;
      float deltaTimeMultiplier = IsCurrentDeviceMouse ? 1.0f : Time.deltaTime;

      _cinemachineTargetYaw += Input.look.x * deltaTimeMultiplier * CinemachineData.Sensativity;
      _cinemachineTargetPitch += Input.look.y * deltaTimeMultiplier * CinemachineData.Sensativity;
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
