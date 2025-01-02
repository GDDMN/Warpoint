using UnityEngine;

public class BotController : MonoBehaviour
{
   [Header("Actor component")]
  [SerializeField] private ActorComponent _actorComponent;
  private GameObject _mainCamera;  
  private CharacterController _controller;

  private Animator _animator;
  private PlayerStateController _stateController = new PlayerStateController();
  private PlayerState _activeState;
  public Animator Animator => _animator;

  private void Awake()
  {
  }

  public void Initialize()
  {
    _animator = _actorComponent.GetComponent<Animator>();
    _controller = _actorComponent.GetComponent<CharacterController>();
    _mainCamera = new GameObject("bot controller camera object");

    _stateController.Initialize(_actorComponent);
    _activeState = _stateController.GetState(PlayerStateType.ALIVE);

    var hurtable = _actorComponent.GetComponent<HurtableActor>();
    hurtable.OnDeath += properties => { StartNewState(PlayerStateType.DEAD); };
    
    StartNewState(PlayerStateType.ALIVE);
    _actorComponent.AssignAnimationIDs();
  }

  public void StartNewState(PlayerStateType type)
  {
    _activeState = _stateController.GetState(type);
    _activeState.Enter(_controller, _mainCamera);
  }

  private void AccesWeaponSettings()
  {
  }

  private void Update()
  {
    if(!_actorComponent.IsAlive)
    {
      return;     
    }

    _activeState.Update();
  }

  private void OnDisable()
  {
    //_actorComponent.OnJumpLounch -= LandingValidate;
    //_controller.OnLanding -= LandingValidate;

    //_actorComponent.Weapon.OnShoot -= ShootingCameraEffect;
  }
}
