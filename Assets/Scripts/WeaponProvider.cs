using UnityEngine;
using System.Collections;
using System;
using TMPro;

public class WeaponProvider : MonoBehaviour
{
  private const float HIPS_COOLDOWN_TIME = 0.13f;
  public string GunplayAnimName; 
  public string ReloadingAnimName;
  public WeaponType weaponType;
  public WeaponData Data;
  public Animator Animator;

  public TrailRenderer BulletTracer;

  public Transform AimTargetObj;

  public Transform RaycastOrigin;

  public ParticleSystem _fireParticles;
  public AudioSource WeaponSounds;

  private bool _alreadyShooting = false;

  private bool _shootValidate = false;
  private Coroutine coroutine;
  private Coroutine cooldownCoroutine = null;
  private Vector3 _direction;

  private float shootingTime = 0f;
  private float spread = 0f;

  public event Action OnShoot;

  private int ammo;

  public int CurrentAmmo => ammo;

  private float _normolizedTime = 0f;

  public float NormolizedShootTime => _normolizedTime;

  public ITimeReceiver _timeReceiver; // Интерфейс для передачи значения

  public event Action OnReload;

    // Интерфейс для приёма значения normalizedTime
  public interface ITimeReceiver
  {
      void UpdateTime(float time);
  }

  public void Initialize(ITimeReceiver receiver)
  {
    ReloadWeapon();
    OnReload += delegate{ ReloadWeapon(); };
    _timeReceiver = receiver;
  }

  public void ReloadWeapon()
  {
    ammo = Data.AmmoCapacity;
  }

  private void OnDisable()
  {
    _alreadyShooting = false;
    _fireParticles.Stop();
    shootingTime = 0f;

    if (coroutine != null)
      StopCoroutine(coroutine);
  }

  private void OnDestroy()
  {
    _alreadyShooting = false;
    _fireParticles.Stop();
    shootingTime = 0f;

    if (coroutine != null)
      StopCoroutine(coroutine);
  }

  public void AtTheReadyPlaySound()
  {
    WeaponSounds.PlayOneShot(Data.AtTheReadySound);
  }

  public void ShootValidate(bool validate, Vector3 direction, bool isAiming)
  {
    if (validate && (isAiming || _shootValidate))
    {
      _direction = direction;
      StartShootRoutine(isAiming);
      _alreadyShooting = true;
    }
    else if(validate && !isAiming)
    {
      if(cooldownCoroutine == null)
        cooldownCoroutine = StartCoroutine(HipsShootingCooldown());
    }
    else
    {
      _alreadyShooting = false;
      _shootValidate = false;

      _fireParticles.Stop();
      Data.Spread.RestartSpread();
      shootingTime = 0f;
      
      if(coroutine != null)
        StopCoroutine(coroutine);

      if(cooldownCoroutine != null)
      {
        StopCoroutine(cooldownCoroutine);
        cooldownCoroutine = null;
      }
    }
  }

  private IEnumerator HipsShootingCooldown()
  {
    Debug.Log("Start Shooting Cooldown");
    yield return new WaitForSeconds(HIPS_COOLDOWN_TIME);
    _shootValidate = true;
    Debug.Log("End Shooting Cooldown");
    
  }

  private void StartShootRoutine(bool isAiming)
  {
    if (_alreadyShooting || ammo <= 0)
      return;

    coroutine = StartCoroutine(ShootRoutine(isAiming));
  }

  private IEnumerator ShootRoutine(bool isAiming)
  {
    while(true)
    {
      Shoot();
      _normolizedTime = 0f;

      while(_normolizedTime < 1f)
      {
        yield return null;
        _normolizedTime += 1f / Data.RecoverySpeed * Time.deltaTime;
        _normolizedTime = Mathf.Clamp(_normolizedTime, 0f, 1f);
        
        _timeReceiver?.UpdateTime(_normolizedTime);
      }
    }
  }

  private void Shoot()
  {
    if (ammo <= 0)
      return;

    Vector3 spread = Data.Spread.GetSpreadPos();

    ammo--;
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

  private void EffectsPlay(Vector3 originPos, Vector3 endPos)
  {
    WeaponSounds.PlayOneShot(Data.ShootSound);
    _fireParticles.Play();
    var trace = Instantiate(BulletTracer, originPos, Quaternion.identity);

    trace.AddPosition(originPos);
    trace.transform.position = endPos;
  }
}
