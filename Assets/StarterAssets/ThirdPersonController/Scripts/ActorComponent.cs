using UnityEngine;
using StarterAssets;
using System;
using UnityEngine.Animations.Rigging;

public class ActorComponent : MonoBehaviour
{
  [SerializeField] private ActorData _data;
  [SerializeField] private WeaponProvider _weaponProvider;

  [Header("Constains")]
  [SerializeField] private TwoBoneIKConstraint _IKConstaint;
  [SerializeField] private MultiAimConstraint _constraintRightHand;
  [SerializeField] private MultiAimConstraint _constaintBack;

  private bool _isAiming = false;
  private bool _isShooting = false;

  // player
  private bool _isAlive = true;
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


  public readonly float LEGS_STEP_SPEED = 0.1f;

  public bool IsAlive => _isAlive;

  public event Action<bool> OnJumpLounch;

  private void Start()
  {
    _hasAnimator = TryGetComponent(out _animator);

    _jumpTimeoutDelta = JumpTimeout;
    _fallTimeoutDelta = FallTimeout;

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
    _isAiming = inputs.aim;

    if (cinemachineData.weaponProvider.weaponType == WeaponType.NO_WEAPON)
      return;

    _animator.SetBool("Aim", inputs.aim);
    _animator.SetInteger("WeaponType", (int)_weaponProvider.weaponType);

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
    float targetSpeed = inputs.sprint ? _data.SprintSpeed : _data.MoveSpeed;

    if (inputs.cruch)
      targetSpeed = _data.CruchSpeed;

    if (cinemachineData.weaponProvider.weaponType != WeaponType.NO_WEAPON)
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
      if (!inputs.aim || cinemachineData.weaponProvider.weaponType == WeaponType.NO_WEAPON)
        transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
    }

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
    if (_isAiming && _data.Grounded)
    {
      ConstaintValidate(true, true);
      return;
    }

    if (!_data.Grounded)
    {
      ConstaintValidate(false, false);
      return;
    }

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

  private void PickUpWeapon(WeaponProvider weaponProvider)
  {
    _weaponProvider = weaponProvider;
    _animator.SetInteger("WeaponType", (int)_weaponProvider.weaponType);
  }

  public void Shooting(StarterAssetsInputs inputs)
  {
    _isShooting = inputs.shooting;
    _isAiming = inputs.aim;

    _weaponProvider.ShootValidate(_isShooting && _isAiming, transform.forward, _animator);
  }
}