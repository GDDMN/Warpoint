using UnityEngine;
using System;

public class HurtableActor : MonoBehaviour, IHurtable
{   
    public event Action<PlayerStateType> OnDeath;
      [SerializeField] private GameObject _prefab;

    [SerializeField] private ActorComponent _actorComponent;
    public void Hurt(int damage)
    {
      _actorComponent.Health -= damage;
      
      if(_actorComponent.Health <= 0 && _actorComponent.ActorValidators.IsAlive)
      {
        Die();
      }
    }

    public void Die()
    {
      OnDeath?.Invoke(PlayerStateType.DEAD);
      _actorComponent.ActorValidators.IsAlive = false;

      _actorComponent.LayersWeightController(false);
      _actorComponent.ConstaintController();
      //_actorComponent.Animator.enabled = false;
    }

    public void Interaction(Vector3 position, int damage)
    {
      var fx = Instantiate(_prefab, position, Quaternion.identity);
      Hurt(damage);
    }

}
