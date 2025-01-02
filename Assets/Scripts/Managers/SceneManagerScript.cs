using System.Collections.Generic;
using StarterAssets;
using UnityEngine;

public class SceneManagerScript : MonoBehaviour
{
    [SerializeField] private GameObject PlayerController;
    [SerializeField] private GameObject BotController;
    [SerializeField] private Transform PlayerSwapnPos;
    [SerializeField] private List<Transform> BotSpawnersPoint;
    private UIMainConteiner uiConteinerInstance;
    private GameObject _player = null;
    private List<GameObject> _bots = new List<GameObject>();

    private ThirdPersonController _playerController = null;
    private ThirdPersonShooterController _shooterController = null;
    private void Start()
    {
        _bots.Clear();
        uiConteinerInstance = UIMainConteiner.Instance;
        uiConteinerInstance.Initialize();

        uiConteinerInstance.GetWindowByType<UIGameHud>().Initialize();

        InitializePlayer();

        for(int i=0;i < BotSpawnersPoint.Count; i++)
        {
            InitializeBot(BotSpawnersPoint[i]);
        }
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

    private void InitializeBot(Transform botSpawnPoint)
    {
        var bot = Instantiate(BotController, botSpawnPoint.position, botSpawnPoint.rotation);
        bot.GetComponent<BotController>().Initialize();

        _bots.Add(bot);
    }

    public void SetBotsAlive()
    {
        foreach(var bot in _bots)
        {
            if(bot.GetComponent<BotController>().StateType != PlayerStateType.ALIVE)
            {
                bot.GetComponent<BotController>().Animator.SetTrigger("AliveTrigger");
                bot.GetComponent<BotController>().StartNewState(PlayerStateType.ALIVE);
            }
        }
    }
    
    private void OnPlayerPickUpWeapon()
    {
        uiConteinerInstance.GetWindowByType<UIGameHud>().weaponBar.SetValuesOfPickedWeapon(_playerController.ActorComponent.Weapon.CurrentAmmo.ToString(),
                                                                                            _playerController.ActorComponent.Weapon.Data.AmmoCapacity.ToString());

        _playerController.ActorComponent.Weapon.OnShoot += (delegate { uiConteinerInstance.GetWindowByType<UIGameHud>().weaponBar.SetActualValueOnShoot(_playerController.ActorComponent.Weapon.CurrentAmmo.ToString()); });
        _playerController.ActorComponent.Weapon.OnReload += (delegate { uiConteinerInstance.GetWindowByType<UIGameHud>().weaponBar.SetActualValueOnShoot(_playerController.ActorComponent.Weapon.CurrentAmmo.ToString()); });
    }
}
