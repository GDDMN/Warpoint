using System;
using UnityEngine;

[Serializable]
public struct CinemachineData
{
  [Header("Cinemachine")]
  [Tooltip("The follow target set in the Cinemachine Virtual Camera that the camera will follow")]
  public GameObject CinemachineCameraTarget;

  [Tooltip("How far in degrees can you move the camera up //Default is 70.0f")]
  public float TopClamp; //Default is 70.0f;

  [Tooltip("How far in degrees can you move the camera down //Default is -30.0f")]
  public float BottomClamp; //Default is -30.0f

  [Tooltip("Additional degress to override the camera. Useful for fine tuning camera position when locked //Default is 0.0f")]
  public float CameraAngleOverride; //Default is 0.0f

  [Tooltip("For locking the camera position on all axis //Default is false")]
  public bool LockCameraPosition; //Default is false

  [Tooltip("//Default is 1f")]
  public float Sensativity; //Default is 1f;
}

