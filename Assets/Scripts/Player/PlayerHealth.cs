using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public float maxHealth = 100f;
    public float currentHealth;
    private AttackManager attackManager;
    public float HealSpell = 25f;
    public int maxHealSpells;
    public int currentHealSpells;
    // Start is called before the first frame update
    void Start()
    {
        currentHealSpells = maxHealSpells;
        currentHealth = maxHealth;
        attackManager = GetComponent<AttackManager>();
    }

    public void Heal(InputAction.CallbackContext context){
        if(context.started){
            if(currentHealSpells > 0 && currentHealth < maxHealth){
                currentHealSpells--;
                currentHealth = Mathf.Min(currentHealth + HealSpell, maxHealth);
                Debug.Log("Healing: " + currentHealSpells);
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
        }
    }
}
