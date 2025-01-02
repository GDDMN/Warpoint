using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponManager : MonoBehaviour
{
  [SerializeField] private List<WeaponProvider> allWeapons = new List<WeaponProvider>();
  [SerializeField] private ActorComponent actor;

  [Header("Effects")]
  [SerializeField] private AudioClip pickUpSound;
  [SerializeField] private AudioSource audioSource;

  private void OnTriggerEnter(Collider other)
  {
    var pickable = other.GetComponent<Pickable>();

    if (pickable == null)
      return;

    foreach (var w in allWeapons)
      w.gameObject.SetActive(false);

    var weapon = allWeapons.Find(w => w.Data.WeaponName == pickable.Name);
    weapon.gameObject.SetActive(true);
    actor.PickUpWeapon(weapon);


    audioSource.PlayOneShot(pickUpSound);
  }
}
