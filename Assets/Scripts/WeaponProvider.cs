using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System;

public class WeaponProvider : MonoBehaviour
{
  public WeaponType weaponType;
  public WeaponData Data;

  public Animator Animator;

  public TrailRenderer BulletTracer;

  public Transform AimTargetObj;

  public Transform RaycastOrigin;

  public ParticleSystem _fireParticles;
  public AudioSource WeaponSounds;

  private bool alreadyShooting = false;
  private Coroutine coroutine;
  private Vector3 _direction;

  private float shootingTime = 0f;
  private float spread = 0f;


  public event Action OnShoot;

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
      _fireParticles.Stop();
      shootingTime = 0f;

      if(coroutine != null)
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
      Vector3 spreading = SpreadPos();
      shootingTime += 0.4f * Time.deltaTime;
      Shoot(spreading);
      yield return new WaitForSecondsRealtime(Data.RecoverySpeed);
    }
  }

  private void Shoot(Vector3 spread)
  {
    RaycastHit hit;
    Ray ray = new Ray();
    ray.origin = RaycastOrigin.position;
    ray.direction = RaycastOrigin.forward + spread;
    
    EffectsPlay(ray.origin, AimTargetObj.position + spread);

    OnShoot?.Invoke();

    if(Physics.Raycast(ray.origin, (AimTargetObj.position + spread) - ray.origin, out hit))
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

  private Vector3 SpreadPos()
  {
    spread = shootingTime * Data.DeltaSpread;
    spread = Mathf.Clamp(spread, 0f, Data.MaxSpread);

    Vector3 spreading = new Vector3(UnityEngine.Random.Range(-spread, spread), UnityEngine.Random.Range(-spread, spread), 0f);
    return spreading;
  }

  private void EffectsPlay(Vector3 originPos, Vector3 endPos)
  {
    WeaponSounds.PlayOneShot(Data.ShootSound);
    _fireParticles.Play();
    var trace = Instantiate(BulletTracer, originPos, Quaternion.identity);

    //Animator.SetTrigger("Shoot");

    trace.AddPosition(originPos);
    trace.transform.position = endPos;
  }
}
