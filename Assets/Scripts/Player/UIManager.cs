using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public List<PowerUI> powerIndicators = new List<PowerUI>();
    public List<Image> powerCharges = new List<Image>();
    public List<Sprite> weaponSprites = new List<Sprite>();
    // public List<Color> powerColors = new List<Color>();
    private Color powerColorIndicator;
    public Image powerIndicator;
    public Image weaponIndicator;
    public Slider healthSlider;
    public Slider rageSlider;
    public Slider screamSlider;
    public Slider visionSlider;
    public Image crossHair;
    private int chargeIndex = 0;
    public TextMeshProUGUI health;
    public Sprite ragePowerSprite;
    public Color ragePowerColorIndicator;
    private int powerIndex = 0;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // powerColorIndicator = powerColors[0];
        UpdateUI();
    }

    public void PowerSpriteIndicatior(int index)
    {
        for (int i = 0; i < powerIndicators.Count; i++)
        {
            if (i == index)
            {
                powerIndicators[i].chargeUI.SetActive(true);
            }
            else
            {
                powerIndicators[i].chargeUI.SetActive(false);
            }
        }
        powerIndicator.sprite = powerIndicators[index].superpowerSprite;
        powerIndex = index;
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

    public void ChangeScream(float current)
    {
        screamSlider.value = current;
    }
    
    public void ChangeVision(float current)
    {
        visionSlider.value = current;
    }

    public void SetMaxPowers(float maxVision, float maxScream)
    {
        visionSlider.maxValue = maxVision;
        screamSlider.maxValue = maxScream;
    }
    
    public void SetMaxPowers(float maxScream)
    {
        screamSlider.maxValue = maxScream;
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

    public void SetHealText(int currentHeal)
    {
        health.text = currentHeal.ToString();
    }

    public void SetRagePower()
    {
        powerIndicator.sprite = ragePowerSprite;
        powerColorIndicator = ragePowerColorIndicator;
        UpdateUI();
    }

    public void EndRagePower()
    {
        // powerIndicator.sprite = powerSprites[powerIndex];
        // powerColorIndicator = powerColors[powerIndex];
        UpdateUI();
    }

    private void UpdateUI()
    {
        powerIndicator.sprite = powerIndicators[powerIndex].superpowerSprite;
        for (int i = 0; i < powerIndicators.Count; i++)
        {
            if (i == powerIndex)
            {
                powerIndicators[i].chargeUI.SetActive(true);
            }
            else
            {
                powerIndicators[i].chargeUI.SetActive(false);
            }
        }
    }
}
