using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class WeaponProvider : MonoBehaviour
{
  public WeaponType weaponType;
  public WeaponData Data;

  public AudioSource WeaponSounds;

  private bool alreadyShooting = false;
  private Coroutine coroutine;
  private Vector3 _direction;

  private Vector2 SCREEN_CENTER_POINT = new Vector2(Screen.width / 2f, Screen.height / 2f);

  public void Initialize()
  {
  }

  public void ShootValidate(bool validate, Vector3 direction)
  {
    if (validate)
    {
      _direction = direction;
      StartShootRoutine();
      alreadyShooting = true;
    }
    else
    {
      alreadyShooting = false;
      StopCoroutine(coroutine);
    }
  }

  private void StartShootRoutine()
  {
    if (alreadyShooting)
      return;

    coroutine = StartCoroutine(ShootRoutine());
  }

  private IEnumerator ShootRoutine()
  {
    while(true)
    {
      Shoot();
      yield return new WaitForSecondsRealtime(Data.RecoverySpeed);
    }
  }

  private void Shoot()
  {
    WeaponSounds.PlayOneShot(Data.ShootSound);

    RaycastHit hit;
    Ray ray = Camera.main.ScreenPointToRay(SCREEN_CENTER_POINT);

    if(Physics.Raycast(ray, out hit, 100f))
    {
      SetDamage(hit);
    }
  }

  private void SetDamage(RaycastHit hit)
  {
    var hurtableObject = hit.collider.GetComponent<IHurtable>();
    if (hurtableObject == null)
      return;

    hurtableObject.Interaction(hit.point, Data.Damage);
  }
}
