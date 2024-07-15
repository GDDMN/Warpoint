using UnityEngine;

public class ActorComponent : MonoBehaviour
{
  [SerializeField] private ActorData _data;

  public ActorData ActorData => _data;

  public void SetGrounded(bool isGrounded)
  {
    _data.Grounded = isGrounded;
  }  

  //public void SetSensativity(float sensativity)
  //{
  //  _data.Sensativity = sensativity;
  //}

}
