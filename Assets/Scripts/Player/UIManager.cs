using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("Power UI")]
    public List<GameObject> powerIndicators = new List<GameObject>();
    public List<Image> powerCharges = new List<Image>();
    // public List<Sprite> weaponSprites = new List<Sprite>();
    // public List<Color> powerColors = new List<Color>();
    private Color powerColorIndicator;
    public Image powerIndicator;
    public Slider screamSlider;
    public Slider visionSlider;
    [Header("Weapon UI")]
    public Image weaponIndicator;
    public Image lightSpecialIndicator;
    public Image heavySpecialIndicator;
    public GameObject specialIndicatorParent;
    [Header("Health/Rage UI")]
    public Slider healthSlider;
    public Slider rageSlider;
    private int chargeIndex = 0;
    public TextMeshProUGUI health;
    public Sprite ragePowerSprite;
    public Color ragePowerColorIndicator;
    private int powerIndex = 0;
    private PlayerStateMachine playerStateMachine;
    [Header("Misc UI")] 
    public GameObject gameplayMenu;
    public GameObject pauseMenu;
    public Image crossHair;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    // void Awake()
    // {
    //     // powerColorIndicator = powerColors[0];
    // }

    public void PowerSpriteIndicatior(int index)
    {
        for (int i = 0; i < powerIndicators.Count; i++)
        {
            if (i == index)
            {
                powerIndicators[i].SetActive(true);
            }
            else
            {
                powerIndicators[i].SetActive(false);
            }
        }
        SetPowerSprite();
        powerIndex = index;
        UpdateUI();
    }

    public void SetPowerSprite()
    {
        powerIndicator.sprite = playerStateMachine.CurrentPower.sprite;
    }
    
    public void WeaponSpriteIndicatior()
    {
        weaponIndicator.sprite = playerStateMachine.CurrentMelee.sprite;
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
        weaponIndicator.sprite = playerStateMachine.CurrentMelee.sprite;
        powerIndicator.sprite = playerStateMachine.CurrentPower.sprite;
        for (int i = 0; i < powerIndicators.Count; i++)
        {
            if (i == powerIndex)
            {
                powerIndicators[i].SetActive(true);
            }
            else
            {
                powerIndicators[i].SetActive(false);
            }
        }
    }

    public void AssignValues()
    {
        playerStateMachine = GetComponent<PlayerStateMachine>();
        UpdateUI();
    }

    public void AssignSpecialAttacks()
    {
        lightSpecialIndicator.sprite = playerStateMachine.CurrentMelee.lightAttack.uiSprite;
        heavySpecialIndicator.sprite = playerStateMachine.CurrentMelee.heavyAttack.uiSprite;
    }

    public void PauseGame(bool paused)
    {
        if (paused)
        {
            pauseMenu.SetActive(true);
            gameplayMenu.SetActive(false);
        }
        else
        {
            pauseMenu.SetActive(false);
            gameplayMenu.SetActive(true);
        }
    }
}
