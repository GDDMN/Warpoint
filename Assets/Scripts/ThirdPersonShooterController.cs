using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using StarterAssets;

public class ThirdPersonShooterController : MonoBehaviour
{
  [SerializeField] private CinemachineVirtualCamera _aimVirtualCamera;
  [SerializeField] private float normalSensativity;
  [SerializeField] private float aimSensativity;
  [SerializeField] private LayerMask aimColliderLayerMask = new LayerMask();

  private ThirdPersonController _controller;
  private StarterAssetsInputs _inputs;
  private Vector2 SCREEN_CENTER_POINT = new Vector2(Screen.width / 2f, Screen.height / 2f);

  private void Awake()
  {
    _controller = GetComponent<ThirdPersonController>();
    _inputs = GetComponent<StarterAssetsInputs>();
  }


  private void Update()
  {
    Vector3 mouseWorldPoint = Vector3.zero;
    
    Ray ray = Camera.main.ScreenPointToRay(SCREEN_CENTER_POINT);
    if (Physics.Raycast(ray, out RaycastHit raycastHit, 999f, aimColliderLayerMask))
      mouseWorldPoint = raycastHit.point;



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
