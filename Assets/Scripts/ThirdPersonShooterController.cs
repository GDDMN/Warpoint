using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using StarterAssets;

public class ThirdPersonShooterController : MonoBehaviour
{
  [SerializeField] private CinemachineVirtualCamera _aimVirtualCamera;

  private StarterAssetsInputs _inputs;

  private void Awake()
  {
    _inputs = GetComponent<StarterAssetsInputs>();
  }


  private void Update()
  {
    if(_inputs.aim)
    {
      _aimVirtualCamera.gameObject.SetActive(true);
    }
    else
    {
      _aimVirtualCamera.gameObject.SetActive(false);
    }
  }
}
