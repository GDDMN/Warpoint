using TMPro;
using UnityEngine;

public class UIWeaponBar : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _actualAmmoCurrentTMPro;
    [SerializeField] private TextMeshProUGUI _fullAmmoCurrentTMPro;

    public void Initialize(string actualAmmoCurrent, string fullAmmoCurrent)
    {
        SetValuesOfPickedWeapon(actualAmmoCurrent, fullAmmoCurrent);
    }

    public void SetValuesOfPickedWeapon(string actualAmmoCurrent, string fullAmmoCurrent)
    {
        _actualAmmoCurrentTMPro.text = actualAmmoCurrent;
        _fullAmmoCurrentTMPro.text = fullAmmoCurrent;
    }

    public void SetActualValueOnShoot(string actualAmmoCurrent)
    {
        _actualAmmoCurrentTMPro.text = actualAmmoCurrent;
    }
}