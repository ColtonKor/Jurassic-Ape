using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public List<Sprite> powerSprites = new List<Sprite>();
    public List<Sprite> weaponSprites = new List<Sprite>();
    public List<Color> powerColors = new List<Color>();
    private Color powerColorIndicator;
    public Image powerIndicator;
    public Image weaponIndicator;
    public Slider healthSlider;
    public Slider rageSlider;
    public Image crossHair;
    public List<Image> powerCharges = new List<Image>();
    private int chargeIndex = 0;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        powerColorIndicator = powerColors[0];
        UpdateUI();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PowerSpriteIndicatior(int index)
    {
        powerIndicator.sprite = powerSprites[index];
        powerColorIndicator = powerColors[index];
        UpdateUI();
    }
    
    public void WeaponSpriteIndicatior(int index)
    {
        weaponIndicator.sprite = weaponSprites[index];
    }
    
    public void SetHealth(float health)
    {
        healthSlider.value = health;
    }

    public void SetMaxHealth(float max)
    {
        healthSlider.maxValue = max;
        SetHealth(max);
    }
    
    public void SetRage(float rage)
    {
        rageSlider.value = rage;
    }

    public void SetMaxRage(float max)
    {
        rageSlider.maxValue = max;
        SetRage(0);
    }

    public void ToggleCrosshair()
    {
        crossHair.gameObject.SetActive(!crossHair.gameObject.activeSelf);
    }

    public void TakePowerCharge()
    {
        powerCharges[chargeIndex].enabled = false;
        chargeIndex++;
    }

    public void AddPowerCharge()
    {
        chargeIndex--;
        powerCharges[chargeIndex].enabled = true;
    }

    private void UpdateUI()
    {
        for (int i = 0; i < powerCharges.Count; i++)
        {
            powerCharges[i].enabled = (i >= chargeIndex);
            powerCharges[i].color = powerColorIndicator;
        }
    }
}
