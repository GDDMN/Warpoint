using UnityEngine;
using Cinemachine;
using StarterAssets;
using System;
using UnityEngine.Animations.Rigging;


public class ThirdPersonShooterController : MonoBehaviour
{
  [Header("Actor component")]
  [SerializeField] private ActorComponent _actorComponent;
  
  [Space(10)]
  //[SerializeField] private Rig aimRig;
  [SerializeField] private CinemachineVirtualCamera _aimVirtualCamera;
  [SerializeField] private float normalSensativity;
  [SerializeField] private float aimSensativity;
  [SerializeField] private Transform _aimObject;

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
  
  private void Aiming()
  {
    //if (weaponProvider.weaponType == WeaponType.NO_WEAPON)
    //  return;

    if (!IsAiming || !_actorComponent.OnGround)
    {
      _aimVirtualCamera.gameObject.SetActive(false);
      _controller.SetSensativity(normalSensativity);
      return;
    }

    //_animator.SetBool("Aim", IsAiming);
    
    
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
