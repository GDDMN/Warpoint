using UnityEngine;
using UnityEngine.InputSystem;

public abstract class PlayerState
{
  public PlayerStateType StateType;

  public ActorComponent ActorComponent;

#if ENABLE_INPUT_SYSTEM && STARTER_ASSETS_PACKAGES_CHECKED
  public PlayerInput PlayerInput;
#endif
  public CharacterController CharacterController;
  
  public GameObject MainCamera;

  public abstract void Initialize(ActorComponent actorComponent);

  public abstract void Enter(CharacterController characterController, GameObject mainCamera);

  public abstract void Update();

  public abstract void Exit();
}