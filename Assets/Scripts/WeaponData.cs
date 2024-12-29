using UnityEngine;
using System;


[Serializable]
public struct WeaponData
{
  public WeaponName WeaponName;
  public WeaponType WeaponType;

  [Header("Combat characteristics")]
  public int AmmoCapacity;
  public float ReloadingSpeed;
  public float RecoverySpeed;
  public float HipMaxSpread;
  public float AimMaxSpread;
  public float DeltaSpread;
  public int Damage;


  [Header("Prefab data")]
  public string PrefabPath;

  [Header("Sound data")]
  public AudioClip ShootSound;
  public AudioClip AtTheReadySound;
  
  [Header("Left hand point")]
  public Transform LeftHandPoint;
}
