using UnityEngine;
using StarterAssets;
using System;
using UnityEngine.Animations.Rigging;
using System.Collections;
using static WeaponProvider;

public class ActorValidators
{
  public bool IsAlive = true;
  public bool IsAiming = false;
  public bool IsShooting = false;
  public bool IsSprint = false;
  public bool IsReloading = false;
  public bool IsCrouch = false;
  public bool IsAnalogMovement = false;

  public Vector2 MoveDirection;
}

public class ActorComponent : MonoBehaviour, ITimeReceiver
{
  private const float HIPS_COOLDOWN_TIME = 0.13f;
  [SerializeField] private ActorData _data;
  [SerializeField] private WeaponProvider _weaponProvider;
  [SerializeField] private RigBuilder rigBuilder;

  [Header("Constains")]
  [SerializeField] private TwoBoneIKConstraint _IKConstaint;
  [SerializeField] private MultiAimConstraint _constraintRightHand;
  [SerializeField] private MultiAimConstraint _constaintBack;

  [SerializeField] private Transform aimObject;

  [SerializeField] private ActorValidators _actorValidators = new ActorValidators();

  private Vector3 RIFLE_CONSTAIN_BODY_OFFSET { get { return new Vector3(-60, -10, 20); } }
  private Vector3 PISTOL_CONSTAIN_BODY_OFFSET { get { return new Vector3(-15, 0, 0); } }

  private float _speed;
  private float _animationBlend;
  private float _targetRotation = 0.0f;
  private float _rotationVelocity = 0.3f;
  private float _verticalVelocity;
  private float _terminalVelocity = 53.0f;

  // animation IDs
  private int _animIDSpeed;
  private int _animIDGrounded;
  private int _animIDJump;
  private int _animIDFreeFall;
  private int _animIDMotionSpeed;

  // timeout deltatime
  private float _jumpTimeoutDelta;
  private float _fallTimeoutDelta;

  private bool _hasAnimator;
  private Animator _animator;

  private Vector2 realDirection = Vector2.zero;
  private Vector2 lastDirection = Vector2.zero;

  public ActorData ActorData => _data;

  public float JumpTimeout => _data.JumpTimeout;

  public float FallTimeout => _data.FallTimeout;

  public bool OnGround => _data.Grounded;

  public WeaponProvider Weapon => _weaponProvider;

  public readonly float LEGS_STEP_SPEED = 0.1f;

  public bool IsAlive => _actorValidators.IsAlive;

  public event Action<bool> OnJumpLounch;

  public event Action<bool> EndReloading;

  public event Action OnWeaponPickUp;

  private float _hipsShootCooldownTime = 1f;
  private float _hipsShootCooldownCurrentTime = 0f;
  private const float _hipsShootCooldownSpeed = 0.5f;

  private float _normolizedTime = 0f;

  private bool _isPlayingShootRoutine = false;


  private bool _shootingIsAvaliable = false;

  private bool _isOnGroundIssue = true;
  private Coroutine _onGRoundRoutineIssue = null;

  private bool _isTheReadyWeaponIssued = false;
  
  private bool _isReloading = false;
  private Coroutine _reloadingCoroutine = null;

  #region VALIDATORS_AND_GETTERS

  public void SetActorStateValidators(bool isAiming, bool isShooting, bool isSprint, bool isCrouch, bool isReloading, Vector2 moveDirection, bool isAnalogMovement)
  {
    _actorValidators.IsAlive = true;
    _actorValidators.IsAiming = isAiming;
    _actorValidators.IsShooting = isShooting;
    _actorValidators.IsSprint = isSprint;
    _actorValidators.IsCrouch = isCrouch;
    _actorValidators.IsReloading = isReloading;

    _actorValidators.MoveDirection = moveDirection;
    _actorValidators.IsAnalogMovement = isAnalogMovement;
  }

  public bool CheckActorDefaultState()
  {
    return _actorValidators.IsAlive && 
          !_actorValidators.IsAiming && 
          !_actorValidators.IsShooting;
  }

  private bool IsAimingActorState()
  {
    return _actorValidators.IsAlive && 
          _actorValidators.IsAiming && 
          !_actorValidators.IsReloading;
  }

  private bool IsShootingActorState()
  {
    return _actorValidators.IsAlive && 
          _actorValidators.IsShooting && 
          !_actorValidators.IsReloading;
  }

  private bool IsCrouchingActorState()
  {
    return _actorValidators.IsAlive && 
          _actorValidators.IsCrouch && 
          !_actorValidators.IsShooting;
  }

  private bool IsSprintingActorState()
  {
    return _actorValidators.IsAlive && 
          _actorValidators.IsSprint && 
          !_actorValidators.IsAiming && 
          !_actorValidators.IsShooting && 
          !_actorValidators.IsCrouch && 
          !_actorValidators.IsReloading;
  }

  private bool IsReloadingState()
  {
    return _actorValidators.IsAlive && 
    _actorValidators.IsReloading &&
    !_actorValidators.IsSprint;
  }
  #endregion

  private void Start()
  {
    _hasAnimator = TryGetComponent(out _animator);

    _jumpTimeoutDelta = JumpTimeout;
    _fallTimeoutDelta = FallTimeout;

    EndReloading += SetReloadingFlag;

    UIMainConteiner.Instance.Initialize();
    UIMainConteiner.Instance.GetWindowByType<UIGameHud>().Initialize();
    PickUpWeapon(_weaponProvider);
  }

  private void Update()
  {
    bool layersWeightValidator = (IsAimingActorState() || IsShootingActorState() || IsReloadingState()) && _data.Grounded;

    LayersWeightController(layersWeightValidator);
    ConstaintController();
  }

  public void SetGrounded(bool isGrounded)
  {
    _data.Grounded = isGrounded;
  }

  public void AssignAnimationIDs()
  {
    _animIDSpeed = Animator.StringToHash("Speed");
    _animIDGrounded = Animator.StringToHash("Grounded");
    _animIDJump = Animator.StringToHash("Jump");
    _animIDFreeFall = Animator.StringToHash("FreeFall");
    _animIDMotionSpeed = Animator.StringToHash("MotionSpeed");
  }

  public void GroundedCheck()
  {
    // set sphere position, with offset
    Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - _data.GroundedOffset,
        transform.position.z);
    _data.Grounded = Physics.CheckSphere(spherePosition, _data.GroundedRadius, _data.GroundLayers,
        QueryTriggerInteraction.Ignore);

    // update animator if using character
    if (_hasAnimator)
    {
      _animator.SetBool(_animIDGrounded, _data.Grounded);
    }

    if(!_data.Grounded)
      OnJumpLounch?.Invoke(false);
  }

  public void Cruch()
  {
    _animator.SetBool("Cruch", _actorValidators.IsCrouch);
  }

  public void Aiming(CinemachineData cinemachineData)
  {
    if (_weaponProvider.weaponType == WeaponType.NO_WEAPON || !_data.Grounded)
      return;

    _animator.SetBool("Aim", _actorValidators.IsAiming);
  }

  public void Shooting()
  {
    if (_data.Grounded)
    {
      _weaponProvider.ShootValidate(IsShootingActorState(), transform.forward, _actorValidators.IsAiming);;
    }
    else
    {
      _weaponProvider.ShootValidate(false, transform.forward, _actorValidators.IsAiming);
    }
    
    PlayShootAnimation();

    if (_actorValidators.IsAiming)
      return;
  }

  private void PlayShootAnimation()
  {
    if(IsAimingActorState() || IsShootingActorState())
    {
      _animator.SetBool("Aim", true);
    }
    else if(!IsAimingActorState())
    {
      _animator.SetBool("Aim", false);
    }

    if(IsShootingActorState() && !_isPlayingShootRoutine && _weaponProvider.CurrentAmmo > 0)
    {
      StartCoroutine(PlayShootAnimRoutine());
    }
  }

  public void UpdateTime(float time)
  {
    _normolizedTime = time;
  }

  private IEnumerator PlayShootAnimRoutine()
  {
    if(!IsAimingActorState())
      yield return new WaitForSeconds(HIPS_COOLDOWN_TIME);

    _isPlayingShootRoutine = true;

    while(_normolizedTime < 1f)
    {
      _normolizedTime = Mathf.Max(_normolizedTime, 0.1f);
      _animator.Play(_weaponProvider.GunplayAnimName, -1, _normolizedTime);
      
      if(!IsShootingActorState())
      {
        _normolizedTime += 1.5f * Time.deltaTime;
      }
      yield return null;
    }

    _animator.Play(_weaponProvider.GunplayAnimName, -1, 1f);
    _isPlayingShootRoutine = false;
  }

  public void LegsMotionValidator()
  {
    Vector2 direction = new Vector2(_actorValidators.MoveDirection.x, _actorValidators.MoveDirection.y);

    if (Vector2.Distance(lastDirection, direction) > 0.01f)
      realDirection = Vector2.Lerp(lastDirection, direction, LEGS_STEP_SPEED);

    realDirection.x = Mathf.Clamp(realDirection.x, -1.0f, 1.0f);
    realDirection.y = Mathf.Clamp(realDirection.y, -1.0f, 1.0f);

    lastDirection = realDirection;

    _animator.SetFloat("MotionX", realDirection.x);
    _animator.SetFloat("MotionZ", realDirection.y);
  }

  public void Move(CinemachineData cinemachineData, CharacterController controller, GameObject mainCamera)
  {
    // set target speed based on move speed, sprint speed and if sprint is pressed
    float targetSpeed = IsSprintingActorState() ? _data.SprintSpeed : _data.MoveSpeed;

    if (_actorValidators.IsCrouch)
      targetSpeed = _data.CruchSpeed;

    if (_weaponProvider.weaponType != WeaponType.NO_WEAPON)
      targetSpeed = _actorValidators.IsAiming ? _data.AimSpeed : targetSpeed;

    // a simplistic acceleration and deceleration designed to be easy to remove, replace, or iterate upon

    // note: Vector2's == operator uses approximation so is not floating point error prone, and is cheaper than magnitude
    // if there is no input, set the target speed to 0
    if (_actorValidators.MoveDirection == Vector2.zero) targetSpeed = 0.0f;

    // a reference to the players current horizontal velocity
    float currentHorizontalSpeed = new Vector3(controller.velocity.x, 0.0f, controller.velocity.z).magnitude;

    float speedOffset = 0.1f;
    float inputMagnitude = _actorValidators.IsAnalogMovement ? _actorValidators.MoveDirection.magnitude : 1f;

    // accelerate or decelerate to target speed
    if (currentHorizontalSpeed < targetSpeed - speedOffset ||
        currentHorizontalSpeed > targetSpeed + speedOffset)
    {
      // creates curved result rather than a linear one giving a more organic speed change
      // note T in Lerp is clamped, so we don't need to clamp our speed
      _speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * inputMagnitude,
          Time.deltaTime * _data.SpeedChangeRate);

      // round speed to 3 decimal places
      _speed = Mathf.Round(_speed * 1000f) / 1000f;
    }
    else
    {
      _speed = targetSpeed;
    }

    _animationBlend = Mathf.Lerp(_animationBlend, targetSpeed, Time.deltaTime * _data.SpeedChangeRate);
    if (_animationBlend < 0.01f) _animationBlend = 0f;

    // normalise input direction
    Vector3 inputDirection = new Vector3(_actorValidators.MoveDirection.x, 0.0f, _actorValidators.MoveDirection.y).normalized;

    // note: Vector2's != operator uses approximation so is not floating point error prone, and is cheaper than magnitude
    // if there is a move input rotate player when the player is moving
    if (_actorValidators.MoveDirection != Vector2.zero)
    {
      _targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg +
                        mainCamera.transform.eulerAngles.y;

      float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, _targetRotation, ref _rotationVelocity,
          _data.RotationSmoothTime);

      // rotate to face input direction relative to camera position
      if ((!_actorValidators.IsAiming || _weaponProvider.weaponType == WeaponType.NO_WEAPON) && !_actorValidators.IsShooting)
        transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
    }

    HipShootingRotate(transform, aimObject);
    Vector3 targetDirection = Quaternion.Euler(0.0f, _targetRotation, 0.0f) * Vector3.forward;

    // move the player
    controller.Move(targetDirection.normalized * (_speed * Time.deltaTime) +
                     new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.deltaTime);

    // update animator if using character
    if (_hasAnimator)
    {
      _animator.SetFloat(_animIDSpeed, _animationBlend);
      _animator.SetFloat(_animIDMotionSpeed, inputMagnitude);
    }
  }

  public void JumpAndGravity(StarterAssetsInputs inputs)
  {
    if (_data.Grounded)
    {
      // reset the fall timeout timer
      _fallTimeoutDelta = _data.FallTimeout;

      // update animator if using character
      if (_hasAnimator)
      {
        _animator.SetBool(_animIDJump, false);
        _animator.SetBool(_animIDFreeFall, false);
      }

      // stop our velocity dropping infinitely when grounded
      if (_verticalVelocity < 0.0f)
      {
        _verticalVelocity = -2f;
      }

      // Jump
      if (inputs.jump && _jumpTimeoutDelta <= 0.0f)
      {
        OnJumpLounch?.Invoke(false);
        // the square root of H * -2 * G = how much velocity needed to reach desired height
        _verticalVelocity = Mathf.Sqrt(_data.JumpHeight * -2f * _data.Gravity);

        // update animator if using character
        if (_hasAnimator)
        {
          _animator.SetBool(_animIDJump, true);
        }
      }

      // jump timeout
      if (_jumpTimeoutDelta >= 0.0f)
      {
        _jumpTimeoutDelta -= Time.deltaTime;
      }
    }
    else
    {
      // reset the jump timeout timer
      _jumpTimeoutDelta = _data.JumpTimeout;

      // fall timeout
      if (_fallTimeoutDelta >= 0.0f)
      {
        _fallTimeoutDelta -= Time.deltaTime;
      }
      else
      {
        // update animator if using character
        if (_hasAnimator)
        {
          _animator.SetBool(_animIDFreeFall, true);
        }
      }

      // if we are not grounded, do not jump
      inputs.jump = false;
    }

    // apply gravity over time if under terminal (multiply by delta time twice to linearly speed up over time)
    if (_verticalVelocity < _terminalVelocity)
    {
      _verticalVelocity += _data.Gravity * Time.deltaTime;
    }
  }

  private void LayersWeightController(bool isLayersActive)
  {

    _animator.SetLayerWeight(2, isLayersActive ? 1 : 0);
    _animator.SetLayerWeight(1, isLayersActive ? 1 : 0);
    _animator.SetLayerWeight(0, isLayersActive ? 0 : 1);
  }

  private void ConstaintController()
  {
    if(_isReloading)
    {
      ConstaintValidate(false, false);
      return;
    }

    if(IsAimingActorState())
    { 
      if(_onGRoundRoutineIssue != null)
        StopCoroutine(_onGRoundRoutineIssue);

      _isOnGroundIssue = true;
      
      ConstaintValidate(true, true);
      return;
    }

    if(IsShootingActorState())
    {
      if(_onGRoundRoutineIssue != null)
        StopCoroutine(_onGRoundRoutineIssue);

      _isOnGroundIssue = true;
      
      ConstaintValidate(true, true);
      return;
    }

    if (!_data.Grounded)
    {
      ConstaintValidate(false, false);
      _isOnGroundIssue = false;
      return;
    }

    ConstaintBodyOffsets(_weaponProvider.weaponType);

    if (_weaponProvider.weaponType != WeaponType.DOUBLE_ARMED)
    {
      ConstaintValidate(false, false);
      return;
    }

    //ConstaintValidate(true, false);
    if(!_isOnGroundIssue && _data.Grounded)
    {
      _onGRoundRoutineIssue = StartCoroutine(WaitForConstainRoutine());
      return;
    }

    if(_data.Grounded && _isOnGroundIssue)
    {
      ConstaintValidate(true, false);
    }
  }

  private void ConstaintValidate(bool lefthandActive, bool armActive)
  {
    _IKConstaint.weight = lefthandActive ? 1f : 0f;
    _constaintBack.weight = armActive ? 1f : 0f;
    _constraintRightHand.weight = armActive ? 1f : 0f;
  }

  private IEnumerator WaitForConstainRoutine()
  {
    yield return new WaitForSeconds(2f);
    //ConstaintValidate(true, false);
    _isOnGroundIssue = true;
  }

  private void ConstaintBodyOffsets(WeaponType type)
  {
    switch (type)
    {
      case WeaponType.ONE_ARMED: _constaintBack.data.offset = PISTOL_CONSTAIN_BODY_OFFSET; break;
      case WeaponType.DOUBLE_ARMED: _constaintBack.data.offset = RIFLE_CONSTAIN_BODY_OFFSET; break;
      default: break;
    }
  }

  public void PickUpWeapon(WeaponProvider weaponProvider)
  { 
    var weaponBar = UIMainConteiner.Instance.GetWindowByType<UIGameHud>().weaponBar;
    rigBuilder.enabled = false;
    
    _weaponProvider = weaponProvider;
    _animator.SetInteger("WeaponType", (int)_weaponProvider.weaponType);

    _IKConstaint.data.target = weaponProvider.Data.LeftHandPoint;
    rigBuilder.enabled = true;

    _weaponProvider.Initialize(this);
    //_weaponProvider.OnShoot += ShootAnimationPlay;
    
    weaponBar.SetValuesOfPickedWeapon(_weaponProvider.CurrentAmmo.ToString(), _weaponProvider.Data.AmmoCapacity.ToString());
    _weaponProvider.OnShoot += delegate { weaponBar.SetActualValueOnShoot(_weaponProvider.CurrentAmmo.ToString()); };
    OnWeaponPickUp?.Invoke();
  }

  private void HipShootingRotate(Transform actorForvard, Transform shootingDirection)
  {
    if (!IsShootingActorState())
      return;

    float angle = Vector3.Angle(shootingDirection.position - actorForvard.position, 
                                actorForvard.forward);

    if (angle < 30f && angle > -30f)
      return;

    Transform lookDirection = new GameObject().transform;
    
    lookDirection.position = new Vector3(shootingDirection.position.x, 
                                         0f, 
                                         shootingDirection.position.z);

    actorForvard.LookAt(lookDirection, Vector3.up);
  }

  public void Reloading()
  {
    if(!IsReloadingState())
      return;

    if(_isReloading)
      return;

    _reloadingCoroutine = StartCoroutine(ReloadingRoutine());
    _animator.Play(_weaponProvider.ReloadingAnimName, 1);
  }

  private IEnumerator ReloadingRoutine()
  {
    _isReloading = true;
    yield return new WaitForSeconds(_weaponProvider.Data.ReloadingSpeed);
    _isReloading = false;
    _weaponProvider.ReloadWeapon();
  }

  private void SetReloadingFlag(bool flag)
  {
    _actorValidators.IsReloading = flag;
  }

}