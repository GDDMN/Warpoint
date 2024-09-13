using UnityEngine;

public abstract class PlayerState
{
  public PlayerStateType StateType;

  public ActorComponent ActorComponent;
  public CinemachineData CinemachineData;
  public CharacterController CharacterController;
  public StarterAssets.StarterAssetsInputs Input;
  public GameObject MainCamera;

  public abstract void Initialize();

  public abstract void Enter(ActorComponent actorComponent, CinemachineData cinemachineData, 
                             CharacterController characterController, StarterAssets.StarterAssetsInputs inputs, 
                             GameObject mainCamera);

  public abstract void Update();

  public abstract void Exit();
}