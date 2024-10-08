using UnityEngine;

public class HurtableCube : MonoBehaviour, IHurtable
{
  [SerializeField] private GameObject _prefab;
  [SerializeField] private AudioSource _coundPlayer;
  [SerializeField] private AudioClip _interactAudioClip;

  public void Die()
  {
  }

  public void Hurt(int damage)
  {
  }

  public void Interaction(Vector3 position, int damage)
  {
    var fx = Instantiate(_prefab, position, Quaternion.identity);
    
    if (_coundPlayer != null)
      _coundPlayer.PlayOneShot(_interactAudioClip);
  }
}
