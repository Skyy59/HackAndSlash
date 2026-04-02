using UnityEngine;
using UnityEngine.UI;

public class HUD_Controller : MonoBehaviour
{
    [SerializeField] private Image styleMeterImage;
    [SerializeField] private Image healthMeterImage;
    [SerializeField] private Image ammoMeterImage;
    [SerializeField] private Image powerMeterImage;

    private void OnEnable() 
    {
        // Style_Controller.OnStyleChange += SetStyleMeter;
        // Player_Health.OnHealthChange += SetHealthMeter;
        // Player_Weapons.OnAmmoChange += SetAmmoMeter;
        // Player_Weapons.OnPowerChange += SetPowerMeter;    
    }

    private void OnDisable() 
    {
        // Style_Controller.OnStyleChange -= SetStyleMeter;
        // Player_Health.OnHealthChange -= SetHealthMeter;
        // Player_Weapons.OnAmmoChange -= SetAmmoMeter;
        // Player_Weapons.OnPowerChange -= SetPowerMeter;    
    }

    public void SetStyleMeter(float _currentStyle, float _totalStyle)
    {
        float _value = _currentStyle / _totalStyle;
        styleMeterImage.fillAmount = _value;

        styleMeterImage.color = Color.Lerp(Color.darkBlue, Color.yellow, _value);

    }

    public void SetHealthMeter(float _currentHealth, float _totalHealth)
    {
        float _value = _currentHealth / _totalHealth;
        healthMeterImage.fillAmount = _value;
    }

    public void SetAmmoMeter(float _currentAmmo, float _totalAmmo)
    {
        float _value = _currentAmmo / _totalAmmo;
        ammoMeterImage.fillAmount = _value;
    }

    public void SetPowerMeter(float _currentPower, float _totalPower)
    {
        float _value = _currentPower / _totalPower;
        powerMeterImage.fillAmount = _value;
    }
}
