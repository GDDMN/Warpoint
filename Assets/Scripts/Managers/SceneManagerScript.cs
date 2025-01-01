using StarterAssets;
using UnityEngine;

public class SceneManagerScript : MonoBehaviour
{
    [SerializeField] private GameObject PlayerController;
    [SerializeField] private Transform PlayerSwapnPos;
    private UIMainConteiner uiConteinerInstance;
    private GameObject _player = null;
    private ThirdPersonController _playerController = null;
    private ThirdPersonShooterController _shooterController = null;
    private void Start()
    {
        uiConteinerInstance = UIMainConteiner.Instance;
        uiConteinerInstance.Initialize();

        uiConteinerInstance.GetWindowByType<UIGameHud>().Initialize();

        InitializePlayer();
    }

    private void InitializePlayer()
    {   
        _player = Instantiate(PlayerController, PlayerSwapnPos.position, PlayerSwapnPos.rotation);

        _playerController = _player.GetComponent<ThirdPersonController>();
        _shooterController = _player.GetComponent<ThirdPersonShooterController>();

        _playerController.Initialize();
        _shooterController.Initialize();
        
        string ammoCopacity = _playerController.ActorComponent.Weapon.Data.AmmoCapacity.ToString();
        string actualAmmoCurrent = _playerController.ActorComponent.Weapon.CurrentAmmo.ToString();


        _playerController.ActorComponent.OnWeaponPickUp += (delegate { OnPlayerPickUpWeapon(); });
        uiConteinerInstance.GetWindowByType<UIGameHud>().weaponBar.SetValuesOfPickedWeapon(actualAmmoCurrent ,ammoCopacity);                                                                                                                                                      
    }

    private void OnPlayerPickUpWeapon()
    {
        uiConteinerInstance.GetWindowByType<UIGameHud>().weaponBar.SetValuesOfPickedWeapon(_playerController.ActorComponent.Weapon.CurrentAmmo.ToString(),
                                                                                            _playerController.ActorComponent.Weapon.Data.AmmoCapacity.ToString());

        _playerController.ActorComponent.Weapon.OnShoot += (delegate { uiConteinerInstance.GetWindowByType<UIGameHud>().weaponBar.SetActualValueOnShoot(_playerController.ActorComponent.Weapon.CurrentAmmo.ToString()); });
    }
}
