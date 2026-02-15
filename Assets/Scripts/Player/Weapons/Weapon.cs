using System;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public float lightDamage;

    public float lightStun;
    
    public float heavyDamage;
    
    public float heavyStun;

    public bool isHeavy;
    
    public bool isCharged;

    public Sprite sprite;
    
    public Blessings lightAttack;
    
    public Blessings heavyAttack;

    public void OnTriggerEnter(Collider other)
    {
        EnemyHealth enemyHealth = other.GetComponent<EnemyHealth>();
        if (enemyHealth != null)
        {
            if (isHeavy)
            {
                enemyHealth.TakeDamage(heavyDamage);
                enemyHealth.TakeStun(heavyStun);
            }
            else
            {
                enemyHealth.TakeDamage(lightDamage);
                enemyHealth.TakeStun(lightStun);
            }
            
        }
    }
}
