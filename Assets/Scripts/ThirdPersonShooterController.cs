using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using StarterAssets;

public enum WeaponType 
{
  NO_WEAPON,
  ONE_ARMED,
  DOUBLE_ARMED
}

public class ThirdPersonShooterController : MonoBehaviour
{
  [SerializeField] private CinemachineVirtualCamera _aimVirtualCamera;
  [SerializeField] private float normalSensativity;
  [SerializeField] private float aimSensativity;

  private ThirdPersonController _controller;
  private StarterAssetsInputs _inputs;
  private Vector2 SCREEN_CENTER_POINT = new Vector2(Screen.width / 2f, Screen.height / 2f);

  private void Awake()
  {
    _controller = GetComponent<ThirdPersonController>();
    _inputs = GetComponent<StarterAssetsInputs>();
  }

  private void OnEnable()
  {

  }

  private void Update()
  {
    Vector3 mouseWorldPoint = Vector3.zero;
    
    Ray ray = Camera.main.ScreenPointToRay(SCREEN_CENTER_POINT);
    mouseWorldPoint = ray.direction * 999f;

    if (_inputs.aim)
    {
      _aimVirtualCamera.gameObject.SetActive(true);
      _controller.SetSensativity(aimSensativity);

      Vector3 worldAimTarget = mouseWorldPoint;
      worldAimTarget.y = transform.position.y;

      Vector3 aimDirection = (worldAimTarget - transform.position).normalized;
      transform.forward = Vector3.Lerp(transform.forward, aimDirection, Time.deltaTime * 20f);
    }
    else
    {
      _aimVirtualCamera.gameObject.SetActive(false);
      _controller.SetSensativity(normalSensativity);
    }
  }
}
