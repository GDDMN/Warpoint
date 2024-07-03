using System.Collections;
using System.Collections.Generic;
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
  [SerializeField] private Rig aimRig;
  [SerializeField] private CinemachineVirtualCamera _aimVirtualCamera;
  [SerializeField] private float normalSensativity;
  [SerializeField] private float aimSensativity;
  [SerializeField] private Transform _aimObject;

  public WeaponProvider weaponProvider;


  private ThirdPersonController _controller;
  private StarterAssetsInputs _inputs;
  private Animator _animator;

  private Vector2 SCREEN_CENTER_POINT = new Vector2(Screen.width / 2f, Screen.height / 2f);

  private void Awake()
  {
    _animator = GetComponent<Animator>();
    _controller = GetComponent<ThirdPersonController>();
    _inputs = GetComponent<StarterAssetsInputs>();
  }

  private void Update()
  {
    Aiming();
  }

  private void Aiming()
  {
    if (weaponProvider.weaponType == WeaponType.NO_WEAPON)
      return;

    //_animator.SetBool("Aim", true);
    _animator.SetBool("Aim", _inputs.aim);
    _animator.SetInteger("WeaponType", (int)weaponProvider.weaponType);

    if (!_inputs.aim )
    {
      _aimVirtualCamera.gameObject.SetActive(false);
      _controller.SetSensativity(normalSensativity);
      aimRig.weight = 0f;
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
    aimRig.weight = 1f;

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
