using System.Collections;
using UnityEngine;
using Cinemachine;

public static class CinemachineShake
{
  private const float shootingShakeIntensity = 0.5f; 

  public static void ShootingShake(CinemachineVirtualCamera camera, float time)
  {
    if (!camera.isActiveAndEnabled)
      return;

    CinemachineBasicMultiChannelPerlin perlinChannel = 
      camera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
    
    perlinChannel.m_AmplitudeGain = shootingShakeIntensity;
    
    camera.StartCoroutine(ShakeTimer(perlinChannel, time));
  }

  private static IEnumerator ShakeTimer(CinemachineBasicMultiChannelPerlin perlinChannel, float time)
  {
    yield return new WaitForSecondsRealtime(time);
    perlinChannel.m_AmplitudeGain = 0f;
  }

}
