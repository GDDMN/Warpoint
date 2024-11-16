using UnityEngine;

public class UIMainConteiner : MonoSingleton<UIMainConteiner>
{
    public UIHealthBar healthBar => _healthBar;
    public UIWeaponBar weaponBar => _weaponBar;

    private RectTransform _rectTransform;

    [SerializeField] private UIHealthBar _healthBar;
    [SerializeField] private UIWeaponBar _weaponBar;

    private void Awake()
    {
        _rectTransform = gameObject.GetComponent<RectTransform>();
    }

    public void Initialize()
    {
        _rectTransform = gameObject.GetComponent<RectTransform>();

        //TODO set active player values
        _healthBar.Initialize(100);
        _weaponBar.Initialize("30", "30");
    }
}
