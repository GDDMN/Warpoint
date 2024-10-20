﻿using UnityEngine;
using StarterAssets;
using System;
using UnityEngine.Animations.Rigging;
using System.Collections;

public class ActorComponent : MonoBehaviour
{
  [SerializeField] private ActorData _data;
  [SerializeField] private WeaponProvider _weaponProvider;
  [SerializeField] private RigBuilder rigBuilder;

  [Header("Constains")]
  [SerializeField] private TwoBoneIKConstraint _IKConstaint;
  [SerializeField] private MultiAimConstraint _constraintRightHand;
  [SerializeField] private MultiAimConstraint _constaintBack;

  [SerializeField] private Transform aimObject;

  private bool _isAiming = false;
  private bool _isShooting = false;

  private Vector3 RIFLE_CONSTAIN_BODY_OFFSET { get { return new Vector3(-60, -10, 20); } }
  private Vector3 PISTOL_CONSTAIN_BODY_OFFSET { get { return new Vector3(-15, 0, 0); } }

  // player
  private bool _isAlive = true;
  private bool _isSprint = false;
  private bool _isReloading = false;

  private float _speed;
  private float _animationBlend;
  private float _targetRotation = 0.0f;
  private float _rotationVelocity;
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

  public bool IsAlive => _isAlive;

  public event Action<bool> OnJumpLounch;

  public event Action<bool> EndReloading;

  public event Action OnWeaponPickUp;

  private void Start()
  {
    _hasAnimator = TryGetComponent(out _animator);

    _jumpTimeoutDelta = JumpTimeout;
    _fallTimeoutDelta = FallTimeout;

    EndReloading += SetReloadingFlag;

    PickUpWeapon(_weaponProvider);
  }

  public void Update()
  {
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

  public void Cruch(StarterAssetsInputs inputs)
  {
    _animator.SetBool("Cruch", inputs.cruch);
  }

  public void Aiming(StarterAssetsInputs inputs, CinemachineData cinemachineData)
  {
    if (_isReloading)
      return;

    _isSprint = false;
    _isAiming = inputs.aim;

    if (_weaponProvider.weaponType == WeaponType.NO_WEAPON)
      return;

    _animator.SetBool("Aim", inputs.aim);
    //_animator.SetInteger("WeaponType", (int)_weaponProvider.weaponType);

    if (!inputs.aim || !_data.Grounded)
    {
      _animator.SetLayerWeight(2, 0);
      _animator.SetLayerWeight(1, 0);
      _animator.SetLayerWeight(0, 1);

      realDirection = Vector2.zero;
      lastDirection = Vector2.zero;

      return;
    }

    _animator.SetLayerWeight(2, 1);
    _animator.SetLayerWeight(1, 1);
    _animator.SetLayerWeight(0, 0);

    Vector2 direction = new Vector2(inputs.move.x, inputs.move.y);

    if (Vector2.Distance(lastDirection, direction) > 0.01f)
      realDirection = Vector2.Lerp(lastDirection, direction, LEGS_STEP_SPEED);

    direction.x = Mathf.Clamp(realDirection.x, -1f, 1f);
    direction.y = Mathf.Clamp(realDirection.y, -1f, 1f);

    lastDirection = realDirection;


    _animator.SetFloat("MotionX", direction.x);
    _animator.SetFloat("MotionZ", direction.y);
  }

  public void Move(StarterAssetsInputs inputs, CinemachineData cinemachineData, CharacterController controller, GameObject mainCamera)
  {
    // set target speed based on move speed, sprint speed and if sprint is pressed
    _isSprint = (inputs.sprint && !_isAiming && !_isShooting);
    float targetSpeed = _isSprint ? _data.SprintSpeed : _data.MoveSpeed;

    if (inputs.cruch)
      targetSpeed = _data.CruchSpeed;

    if (_weaponProvider.weaponType != WeaponType.NO_WEAPON)
      targetSpeed = inputs.aim ? _data.AimSpeed : targetSpeed;

    // a simplistic acceleration and deceleration designed to be easy to remove, replace, or iterate upon

    // note: Vector2's == operator uses approximation so is not floating point error prone, and is cheaper than magnitude
    // if there is no input, set the target speed to 0
    if (inputs.move == Vector2.zero) targetSpeed = 0.0f;

    // a reference to the players current horizontal velocity
    float currentHorizontalSpeed = new Vector3(controller.velocity.x, 0.0f, controller.velocity.z).magnitude;

    float speedOffset = 0.1f;
    float inputMagnitude = inputs.analogMovement ? inputs.move.magnitude : 1f;

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
    Vector3 inputDirection = new Vector3(inputs.move.x, 0.0f, inputs.move.y).normalized;

    // note: Vector2's != operator uses approximation so is not floating point error prone, and is cheaper than magnitude
    // if there is a move input rotate player when the player is moving
    if (inputs.move != Vector2.zero)
    {
      _targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg +
                        mainCamera.transform.eulerAngles.y;

      float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, _targetRotation, ref _rotationVelocity,
          _data.RotationSmoothTime);

      // rotate to face input direction relative to camera position
      if ((!inputs.aim || _weaponProvider.weaponType == WeaponType.NO_WEAPON) && !_isShooting)
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

  private void ConstaintController()
  {

    if (_isReloading)
    {
      ConstaintValidate(false, false);
      return;
    }

    if (_isAiming && _data.Grounded && !_isSprint)
    {
      ConstaintValidate(true, true);
      return;
    }

    if (_isShooting && _data.Grounded && !_isSprint)
    {
      ConstaintValidate(true, true);
      return;
    }

    if (!_data.Grounded)
    {
      ConstaintValidate(false, false);
      return;
    }

    ConstaintBodyOffsets(_weaponProvider.weaponType);

    if (_weaponProvider.weaponType != WeaponType.DOUBLE_ARMED)
    {
      ConstaintValidate(false, false);
      return;
    }

    ConstaintValidate(true, false);
  }

  private void ConstaintValidate(bool lefthandActive, bool armActive)
  {
    _IKConstaint.weight = lefthandActive ? 1f : 0f;
    _constaintBack.weight = armActive ? 1f : 0f;
    _constraintRightHand.weight = armActive ? 1f : 0f;
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
    rigBuilder.enabled = false;

    _weaponProvider = weaponProvider;
    _animator.SetInteger("WeaponType", (int)_weaponProvider.weaponType);

    _IKConstaint.data.target = weaponProvider.Data.LeftHandPoint;
    rigBuilder.enabled = true;

    _weaponProvider.Initialize();
    _weaponProvider.OnShoot += ShootAnimationPlay;
    OnWeaponPickUp?.Invoke();
  }

  private void ShootAnimationPlay()
  {
    _animator.SetTrigger("Shoot");
  }

  public void Shooting(StarterAssetsInputs inputs)
  {
    if (_isReloading || _isSprint || !_data.Grounded)
      return;

    _isShooting = inputs.shooting;
    bool aiming = inputs.aim;

    if (!_isShooting)
    {
      _weaponProvider.ShootValidate(false, transform.forward, aiming);
      return;
    }

    if (_data.Grounded)
    {
      _weaponProvider.ShootValidate(_isShooting, transform.forward, aiming);
    }
    else
    {
      _weaponProvider.ShootValidate(false, transform.forward, aiming);
    }

    if (_isAiming)
      return;

    _animator.SetLayerWeight(2, 1);
    _animator.SetLayerWeight(1, 1);
    _animator.SetLayerWeight(0, 0);

    Vector2 direction = new Vector2(inputs.move.x, inputs.move.y);

    _animator.SetFloat("MotionX", direction.x);
    _animator.SetFloat("MotionZ", direction.y);
  }

  private void HipShootingRotate(Transform actorForvard, Transform shootingDirection)
  {
    if (!_isShooting)
      return;

    float angle = Vector3.Angle(shootingDirection.position - actorForvard.position, 
                                actorForvard.forward);

    Debug.Log(angle);

    if (angle < 60f && angle > -60f)
      return;


    Transform lookDirection = new GameObject().transform;
    
    lookDirection.position = new Vector3(shootingDirection.position.x, 
                                         0f, 
                                         shootingDirection.position.z);

    actorForvard.LookAt(lookDirection, Vector3.up);
  }

  public void Reloading(StarterAssetsInputs inputs)
  {
    //var reloading = inputs.reloading;
    //SetReloadingFlag(true);
    //
    //if (reloading)
    //  StartCoroutine(ReloadRoutine());
  }

  private IEnumerator ReloadRoutine()
  {
    _animator.SetTrigger("Reloading");
    yield return new WaitForSecondsRealtime(2f);

    EndReloading?.Invoke(false);
  }

  private void SetReloadingFlag(bool flag)
  {
    _isReloading = flag;
  }
}