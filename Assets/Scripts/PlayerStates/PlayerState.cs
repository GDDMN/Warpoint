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
  public StarterAssets.StarterAssetsInputs Input;
  public GameObject MainCamera;

  public abstract void Initialize();

  public abstract void Enter(ActorComponent actorComponent, CinemachineData cinemachineData, 
                             CharacterController characterController, StarterAssets.StarterAssetsInputs inputs, 
                             GameObject mainCamera, PlayerInput playerInput);

  public abstract void Update();

  public abstract void LateUpdate();

  public abstract void Exit();
}