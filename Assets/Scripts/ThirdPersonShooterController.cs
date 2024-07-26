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

  private void Update()
  {
    OnGround = _actorComponent.ActorData.Grounded;
    IsAiming = _inputs.aim;

    ConstaintController();
    Aiming();
  }

  private void ConstaintController()
  {
    if (IsAiming)
    {
      ConstaintValidate(true);
      _constaintBack.weight = 1f;
      _constraintRightHand.weight = 1f;
      aimRig.weight = 1f;
      return;
    }

    if(!OnGround)
    {
      ConstaintValidate(false);
      aimRig.weight = 0f;
      _constaintBack.weight = 0f;
      _constraintRightHand.weight = 0f;
      return;
    }

    if (weaponProvider.weaponType != WeaponType.DOUBLE_ARMED)
    {
      ConstaintValidate(false);
      aimRig.weight = 0f;
      _constaintBack.weight = 0f;
      _constraintRightHand.weight = 0f;
      return;
    }

    ConstaintValidate(true);
    aimRig.weight = 1f;
    _constaintBack.weight = 0f;
    _constraintRightHand.weight = 0f;
  }

  private void ConstaintValidate(bool active)
  {
    _IKConstaint.weight = active? 1f : 0f;
  }

  private void Aiming()
  {
    if (weaponProvider.weaponType == WeaponType.NO_WEAPON)
      return;

    _animator.SetBool("Aim", IsAiming);
    _animator.SetInteger("WeaponType", (int)weaponProvider.weaponType);

    if (!IsAiming)
    {
      _aimVirtualCamera.gameObject.SetActive(false);
      _controller.SetSensativity(normalSensativity);
      return;
    }

    
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
