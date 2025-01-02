using UnityEngine;
using System;
using System.Collections;

public class HurtableActor : MonoBehaviour, IHurtable
{   
    public event Action<PlayerStateType> OnDeath;
    [SerializeField] private GameObject _prefab;

    [SerializeField] private ActorComponent _actorComponent;
    
    private CharacterController characterController;

    private void Start()
    {
        characterController = GetComponent<CharacterController>();
        DisableRagdoll();
    }

    public void InitializeAliveState()
    { 
        characterController.enabled = true;
        characterController.detectCollisions = true;
        _actorComponent.Animator.enabled = true;
        DisableRagdoll();
    }

    public void Hurt(int damage)
    {
      _actorComponent.Health -= damage;
      
      if(_actorComponent.Health <= 0 && _actorComponent.ActorValidators.IsAlive)
      {
        Die();
      }
    }

    private void DisableRagdoll()
    {
        Rigidbody[] rigidbodies = GetComponentsInChildren<Rigidbody>();

        foreach (Rigidbody rb in rigidbodies)
        {
            rb.isKinematic = true; // Отключаем физику
            rb.gameObject.GetComponent<Collider>().enabled = false;
        }
    }

    private IEnumerator EnableRagdoll()
    {
        yield return new WaitForSeconds(1.3f);

        characterController.enabled = false;
        characterController.detectCollisions = false;
        _actorComponent.Animator.enabled = false;

        Rigidbody[] rigidbodies = GetComponentsInChildren<Rigidbody>(true);
        Collider[] colliders = GetComponentsInChildren<Collider>(true);

        foreach(var col in colliders)
        {
            col.enabled = true;
        }

        foreach (Rigidbody rb in rigidbodies)
        {
            rb.isKinematic = false;
        }
    }

    public void Die()
    {
      OnDeath?.Invoke(PlayerStateType.DEAD);
      _actorComponent.ActorValidators.IsAlive = false;

      _actorComponent.LayersWeightController(false);
      _actorComponent.ConstaintController();

      StartCoroutine(EnableRagdoll());
      //_actorComponent.Animator.enabled = false;
    }

    public void Interaction(Vector3 position, int damage)
    {
      var fx = Instantiate(_prefab, position, Quaternion.identity);
      Hurt(damage);
    }

}
