using System.Collections;
using UnityEngine;
using Cinemachine;

public static class CinemachineShake
{
  private const float shootingShakeIntensity = 0.5f;

  private const float shootingTimeShake = 0.1f;

  public static void ShootingShake(CinemachineVirtualCamera camera, float time)
  {
    if (!camera.isActiveAndEnabled)
      return;

    CinemachineBasicMultiChannelPerlin perlinChannel = 
      camera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
    
    perlinChannel.m_AmplitudeGain = shootingShakeIntensity;
    
    camera.StartCoroutine(ShakeTimer(perlinChannel));
  }

  private static IEnumerator ShakeTimer(CinemachineBasicMultiChannelPerlin perlinChannel)
  {
    yield return new WaitForSecondsRealtime(shootingTimeShake);
    perlinChannel.m_AmplitudeGain = 0f;
  }

}
