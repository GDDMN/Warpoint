using UnityEngine;
using Cinemachine;
using StarterAssets;
using System;
using UnityEngine.Animations.Rigging;

public enum WeaponType 
{
  NO_WEAPON,
  ONE_ARMED,
  DOUBLE_ARMED,
  THROWABLE
}

[Serializable]
public struct WaponData
{
  public WeaponType Type;
}


public class ThirdPersonShooterController : MonoBehaviour
{
  [Header("Actor component")]
  [SerializeField] private ActorComponent _actorComponent;
  
  [Space(10)]
  [SerializeField] private Rig aimRig;
  [SerializeField] private CinemachineVirtualCamera _aimVirtualCamera;
  [SerializeField] private float normalSensativity;
  [SerializeField] private float aimSensativity;
  [SerializeField] private Transform _aimObject;

  [SerializeField] private TwoBoneIKConstraint _IKConstaint;
  [SerializeField] private MultiAimConstraint _constraintRightHand;
  [SerializeField] private MultiAimConstraint _constaintBack;

  public WeaponProvider weaponProvider;


  private ThirdPersonController _controller;
  private StarterAssetsInputs _inputs;
  private Animator _animator;

  private bool IsAiming = false;
  private bool OnGround = true;
  private Vector2 SCREEN_CENTER_POINT = new Vector2(Screen.width / 2f, Screen.height / 2f);

  private void Awake()
  {
    _animator = GetComponent<Animator>();
    _controller = GetComponent<ThirdPersonController>();
    _inputs = GetComponent<StarterAssetsInputs>();
  }

  private void Start()
  {
    _actorComponent.OnJumpLounch += LandingValidate;
    _controller.OnLanding += LandingValidate;
  }

  private void Update()
  {
    IsAiming = _inputs.aim;

    Aiming();
    ConstaintController();
  }

  private void OnDisable()
  {
    _actorComponent.OnJumpLounch -= LandingValidate;
    _controller.OnLanding -= LandingValidate;
  }

  private void LandingValidate(bool OnLand)
  {
    OnGround = OnLand;
  }

  private void ConstaintController()
  {
    if (IsAiming && OnGround)
    {
      ConstaintValidate(true, true);
      return;
    }

    if(!OnGround)
    {
      ConstaintValidate(false, false);
      //aimRig.weight = 0f;
      return;
    }

    if (weaponProvider.weaponType != WeaponType.DOUBLE_ARMED)
    {
      ConstaintValidate(false, false);
      //aimRig.weight = 0f;
      return;
    }

    ConstaintValidate(true, false);
    //aimRig.weight = 1f;
  }

  private void ConstaintValidate(bool lefthandActive, bool armActive)
  {
    _IKConstaint.weight = lefthandActive ? 1f : 0f;
    _constaintBack.weight = armActive ? 1f : 0f;
    _constraintRightHand.weight = armActive ? 1f : 0f;
  }

  private void Aiming()
  {
    if (weaponProvider.weaponType == WeaponType.NO_WEAPON)
      return;

    if (!IsAiming || !OnGround)
    {
      _aimVirtualCamera.gameObject.SetActive(false);
      _controller.SetSensativity(normalSensativity);
      return;
    }

    _animator.SetBool("Aim", IsAiming);
    _animator.SetInteger("WeaponType", (int)weaponProvider.weaponType);
    
    Vector3 mouseWorldPoint = Vector3.zero;
    Ray ray = Camera.main.ScreenPointToRay(SCREEN_CENTER_POINT);
    mouseWorldPoint = ray.direction * 999f;

    _aimObject.position = mouseWorldPoint;

    _aimVirtualCamera.gameObject.SetActive(true);
    _controller.SetSensativity(aimSensativity);

    Vector3 worldAimTarget = mouseWorldPoint;
    worldAimTarget.y = transform.position.y;

    Vector3 aimDirection = (worldAimTarget - transform.position).normalized;
    transform.forward = Vector3.Lerp(transform.forward, aimDirection, Time.deltaTime * 20f);

    Shooting();
  }

 
  private void Shooting()
  {
    if (!_inputs.shooting)
    {
      _animator.SetBool("Shoot", false);
      return;
    }

    _animator.SetBool("Shoot", true);

  }
}
