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

  [Header("Spread sprite")]
  public Spread Spread;
  public float SpreadFactor;

  public int Damage;


  [Header("Prefab data")]
  public string PrefabPath;

  [Header("Sound data")]
  public AudioClip ShootSound;
  public AudioClip AtTheReadySound;

  [Header("Left hand point")]
  public Transform LeftHandPoint;
}

[Serializable]
public class Spread
{
  [SerializeField] private Vector2[] _spreadPos;
  private int _iterator;
  public Vector2 GetSpreadPos()
  {
    Vector2 value = _spreadPos[_iterator];
    _iterator++;
    
    if(_iterator >= _spreadPos.Length)
      _iterator = 0;

    return value;
  }
}
