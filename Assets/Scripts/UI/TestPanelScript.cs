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
        UIMainConteiner.Instance.Initialize();
        var window = UIMainConteiner.Instance.GetWindowByType<UIGameHud>();

        _checkSliderUp.onClick.AddListener(delegate {window.healthBar.SetSliderValue(100); });
        _checkSliderDown.onClick.AddListener(delegate {window.healthBar.SetSliderValue(10); });

        _checkReload.onClick.AddListener(delegate {actualAmmo = 30; });                                                                                        
        _checkReload.onClick.AddListener(delegate {window.weaponBar.SetValuesOfPickedWeapon(actualAmmo.ToString(),
                                                                                            actualAmmo.ToString()); });

        _checkShoot.onClick.AddListener(delegate {actualAmmo--; });
        _checkShoot.onClick.AddListener(delegate {window.weaponBar.SetActualValueOnShoot(actualAmmo.ToString()); });
   }
}
