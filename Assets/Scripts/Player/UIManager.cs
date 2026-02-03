using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public List<Sprite> powerSprites = new List<Sprite>();
    public List<GameObject> powerChargeBars = new List<GameObject>();
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
    public TextMeshProUGUI health;
    public Sprite ragePowerSprite;
    public Color ragePowerColorIndicator;
    private int powerIndex;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        powerColorIndicator = powerColors[0];
        UpdateUI();
    }

    public void PowerSpriteIndicatior(int index)
    {
        for (int i = 0; i < powerChargeBars.Count; i++)
        {
            if (i == index)
            {
                powerChargeBars[i].SetActive(true);
            }
            else
            {
                powerChargeBars[i].SetActive(false);
            }
        }
        powerIndicator.sprite = powerSprites[index];
        // powerColorIndicator = powerColors[index];
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
        powerIndicator.sprite = powerSprites[powerIndex];
        powerColorIndicator = powerColors[powerIndex];
        UpdateUI();
    }

    private void UpdateUI()
    {
        for (int i = 0; i < powerCharges.Count; i++)
        {
            powerCharges[i].enabled = (i >= chargeIndex);
            // powerCharges[i].color = powerColorIndicator;
        }
    }
}
