using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class TestPanelScript : MonoBehaviour
{
    [SerializeField] private Button _checkSliderUp;
    [SerializeField] private Button _checkSliderDown;

    [SerializeField] private Button _checkShoot;
    [SerializeField] private Button _checkReload;


    private int actualAmmo = 30;
    private void Start()
    {
        var instance = UIMainConteiner.Instance;
        
        instance.Initialize();

        _checkSliderUp.onClick.AddListener(delegate {instance.healthBar.SetSliderValue(100); });
        _checkSliderDown.onClick.AddListener(delegate {instance.healthBar.SetSliderValue(10); });

        _checkReload.onClick.AddListener(delegate {actualAmmo = 30; });                                                                                        
        _checkReload.onClick.AddListener(delegate {instance.weaponBar.SetValuesOfPickedWeapon(actualAmmo.ToString(),
                                                                                            actualAmmo.ToString()); });

        _checkShoot.onClick.AddListener(delegate {actualAmmo--; });
        _checkShoot.onClick.AddListener(delegate {instance.weaponBar.SetActualValueOnShoot(actualAmmo.ToString()); });
   }
}
