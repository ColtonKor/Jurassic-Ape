using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    public float maxHealth = 100f;
    public float currentHealth;
    private AttackManager attackManager;
    public float healSpell = 25f;
    public int maxHealSpells;
    public int currentHealSpells;
    public float maxRage;
    public float currentRage;
    private UIManager uIManager;
    // Start is called before the first frame update
    void Start()
    {
        currentHealSpells = maxHealSpells;
        currentHealth = maxHealth;
        attackManager = GetComponent<AttackManager>();
        uIManager = GetComponent<UIManager>();
        uIManager.SetMaxHealth(maxHealth);
        uIManager.SetMaxRage(maxRage);
    }

    public void Heal(InputAction.CallbackContext context){
        if(context.started){
            if(currentHealSpells > 0 && currentHealth < maxHealth){
                currentHealSpells--;
                currentHealth = Mathf.Min(currentHealth + healSpell, maxHealth);
                Debug.Log("Healing: " + currentHealSpells);
                uIManager.SetHealth(currentHealth);
            }
        }
    }
    
    public void RestHeal() {
        currentHealth = maxHealth;
        currentHealSpells = maxHealSpells;
    }

    public void TakeDamage(float damage){
        if(!attackManager.shieldActive){
            currentHealth -= damage;
            uIManager.SetHealth(currentHealth);
        }
    }
}
