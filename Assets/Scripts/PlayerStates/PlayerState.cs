using UnityEngine;
using UnityEngine.InputSystem;

public abstract class PlayerState
{
  public PlayerStateType StateType;

  public ActorComponent ActorComponent;
  public CinemachineData CinemachineData;

#if ENABLE_INPUT_SYSTEM && STARTER_ASSETS_PACKAGES_CHECKED
  public PlayerInput PlayerInput;
#endif
  public CharacterController CharacterController;
  
  public GameObject MainCamera;

  public abstract void Initialize();

  public abstract void Enter(ActorComponent actorComponent, CinemachineData cinemachineData, 
                             CharacterController characterController, GameObject mainCamera);

  public abstract void Update(bool isAiming, bool isShooting, bool isSprint, 
                            bool isCrouch, bool isReloading, Vector2 moveDirection, 
                            bool isAnalogMovement, bool isJump);

  public abstract void LateUpdate(Vector2 look);

  public abstract void Exit();
}