using UnityEngine;

public class BotController : MonoBehaviour
{
   [Header("Actor component")]
  [SerializeField] private ActorComponent _actorComponent;
  private GameObject _mainCamera;  
  private CharacterController _controller;
  //[Header("Weapon")]
  //[SerializeField] private WeaponProvider weapon;

  //private ThirdPersonController _controller;
  private Animator _animator;
  [SerializeField] private CinemachineData _cinemachineData;
  private PlayerStateController _stateController = new PlayerStateController();
  private PlayerState _activeState;

  private bool IsAiming = false;
  private bool OnGround = true;

  private void Awake()
  {
    _animator = GetComponent<Animator>();
    _controller = GetComponent<CharacterController>();
    _mainCamera = new GameObject("bot controller camera object");
  }

  private void Start()
  {
    _stateController.Initialize();
    _activeState = _stateController.GetState(PlayerStateType.ALIVE);

    //_actorComponent.OnJumpLounch += LandingValidate;
    //_actorComponent.OnWeaponPickUp += AccesWeaponSettings;
    //_controller.OnLanding += LandingValidate;
    _activeState.Enter(_actorComponent, _cinemachineData, _controller, _mainCamera);
    _actorComponent.AssignAnimationIDs();
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

    //bool isJump = false;
    _activeState.Update(false, false, false, false, false, Vector2.zero, true, false);
  }

  //private void LateUpdate()
  //{
  //  _activeState.LateUpdate(Vector3.forward);
  //}

  private void OnDisable()
  {
    _actorComponent.OnJumpLounch -= LandingValidate;
    //_controller.OnLanding -= LandingValidate;

    //_actorComponent.Weapon.OnShoot -= ShootingCameraEffect;
  }

  private void LandingValidate(bool OnLand)
  {
    OnGround = OnLand;
  }
}
