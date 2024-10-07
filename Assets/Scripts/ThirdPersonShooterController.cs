using UnityEngine;
using Cinemachine;
using StarterAssets;
using System;
using UnityEngine.Animations.Rigging;
using DG.Tweening;

public class ThirdPersonShooterController : MonoBehaviour
{
  [Header("Actor component")]
  [SerializeField] private ActorComponent _actorComponent;
  
  [Space(10)]
  [SerializeField] private CinemachineVirtualCamera _aimVirtualCamera;
  [SerializeField] private float normalSensativity;
  [SerializeField] private float aimSensativity;
  [SerializeField] private Transform _aimObject;
  [SerializeField] private Transform _aimCamObj;

  [Header("Weapon")]
  [SerializeField] private WeaponProvider weapon;

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

    weapon.OnShoot += ShootingCameraEffect;
  }

  private void Update()
  {
    IsAiming = _inputs.aim;
    Aiming();
  }

  private void OnDisable()
  {
    _actorComponent.OnJumpLounch -= LandingValidate;
    _controller.OnLanding -= LandingValidate;

    weapon.OnShoot -= ShootingCameraEffect;
  }

  private void LandingValidate(bool OnLand)
  {
    OnGround = OnLand;
  }
  
  private void ShootingCameraEffect()
  {
    CinemachineShake.ShootingShake(_aimVirtualCamera, weapon.Data.RecoverySpeed);

    //var cameraTransform = _aimCamObj;
    //
    //cameraTransform
    //  .DOShakePosition(0.15f, 1f, 10, 90f, false, true, ShakeRandomnessMode.Harmonic)
    //  .SetEase(Ease.InOutBounce);
  }

  private void Aiming()
  {
    if (!IsAiming || !_actorComponent.OnGround)
    {
      _aimVirtualCamera.gameObject.SetActive(false);
      _controller.SetSensativity(normalSensativity);
      return;
    }

    Vector3 mouseWorldPoint = Vector3.zero;
    Vector2 centerPoint = new Vector2(Screen.width / 2f, Screen.height / 2f);
    Ray ray = Camera.main.ScreenPointToRay(centerPoint);
    mouseWorldPoint = ray.direction * 999f;

    _aimObject.position = mouseWorldPoint;

    _aimVirtualCamera.gameObject.SetActive(true);
    _controller.SetSensativity(aimSensativity);

    Vector3 worldAimTarget = mouseWorldPoint;
    worldAimTarget.y = transform.position.y;

    Vector3 aimDirection = (worldAimTarget - transform.position).normalized;
    transform.forward = Vector3.Lerp(transform.forward, aimDirection, Time.deltaTime * 20f);
  }
}
