using UnityEngine;

public class UIGameHud : UIBaseWindow
{
    public UIHealthBar healthBar => _healthBar;
    public UIWeaponBar weaponBar => _weaponBar;

    [SerializeField] private UIHealthBar _healthBar;
    [SerializeField] private UIWeaponBar _weaponBar;

    private RectTransform _rectTransform;
    private void Awake()
    {
        _rectTransform = gameObject.GetComponent<RectTransform>();
    }


    //TODO set active player values
    public void Initialize()
    {


        if(isAlreadyInitialized)
            return;

        _rectTransform = gameObject.GetComponent<RectTransform>();
        InitializeElements(100, "30", "30");
    }

    private void InitializeElements(int health, string actualAmmoCountString, string fullAmmoCountString)
    {
        _healthBar.Initialize(health);
        _weaponBar.Initialize(actualAmmoCountString, fullAmmoCountString);
    }
}
