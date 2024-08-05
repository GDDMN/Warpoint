using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Configs/Data bases/Weapons Database", fileName = "Weapons Database")]
public class WeaponsDatabase : ScriptableObject
{
  [SerializeField] private List<WeaponScriptableObject> _allWEapons;

  public WeaponScriptableObject GetWeapon(WeaponName name)
  {
    return _allWEapons.Find(weapon => weapon.WeaponData.WeaponName == name);
  }
}

