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

  private void Awake()
  {
    _animator = GetComponent<Animator>();
    _controller = GetComponent<CharacterController>();
    _mainCamera = new GameObject("bot controller camera object");
  }

  private void Start()
  {
    _stateController.Initialize(_actorComponent);
    _activeState = _stateController.GetState(PlayerStateType.ALIVE);
    _actorComponent.OnDeath += (delegate { StartNewState(PlayerStateType.DEAD); });

    _activeState.Enter(_controller, _mainCamera);
    _actorComponent.AssignAnimationIDs();
  }

  private void StartNewState(PlayerStateType type)
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
