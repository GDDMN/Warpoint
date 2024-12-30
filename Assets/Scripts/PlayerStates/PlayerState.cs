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

  public abstract void Initialize();

  public abstract void Enter(ActorComponent actorComponent, 
                             CharacterController characterController, GameObject mainCamera);

  public abstract void Update();

  public abstract void Exit();
}