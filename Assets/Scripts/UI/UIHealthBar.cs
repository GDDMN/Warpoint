using UnityEngine;
using UnityEngine.UI;

public class UIHealthBar : MonoBehaviour
{
    private Slider m_healthSlider;

    private void Awake()
    {
        m_healthSlider = gameObject.GetComponent<Slider>();
    }

    private void OnEnable()
    {
    }

    private void OnDisable()
    {
    }

    public void Initialize(int value)
    {
        SetSliderValue(value);
    }

    public void SetSliderValue(int value)
    {
        m_healthSlider.value = ConvertFromDamageToSliderValue(value);
    }

    private float ConvertFromDamageToSliderValue(int value)
    {
        return value / 100f;
    }
}