using UnityEngine;


public interface IHurtable
{
  void Hurt(int damage);

  void Die();

  void Interaction(Vector3 position, int damage);
}
