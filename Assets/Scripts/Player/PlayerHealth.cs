using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    public float maxHealth = 100f;
    [HideInInspector] public float currentHealth;
    private AttackManager attackManager;
    public float healSpell = 25f;
    public int maxHealSpells;
    [HideInInspector] public int currentHealSpells;
    public float maxRage;
    [HideInInspector] public float currentRage;
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
        uIManager.SetHealText(currentHealSpells);
    }

    public void Heal(){
        currentHealSpells--;
        currentHealth = Mathf.Min(currentHealth + healSpell, maxHealth);
        Debug.Log("Healing: " + currentHealSpells); 
        uIManager.SetHealth(currentHealth);
        uIManager.SetHealText(currentHealSpells);
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

    public void ActivateRage()
    {
        uIManager.SetRagePower();
    }
    
    public void EndRage()
    {
        uIManager.EndRagePower();
    }
}
