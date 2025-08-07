using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SupernovaSpell : Spells
{
    [Header("Supernova Explosion Light Stats")]
    public float supernovaLightDamage;
    public float supernovaLightFire;
    public int supernovaLightPerSeconds;
    [Header("Supernova Explosion Heavy Stats")]
    public float supernovaHeavyDamage;
    public float supernovaHeavyFire;
    public int supernovaHeavyPerSeconds;
    public GameObject supernovaPrefab;
    void OnTriggerEnter(Collider collision){
        if(collision.isTrigger){
            return;
        }
        Quaternion adjustedRotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0);
        GameObject explosion = Instantiate(supernovaPrefab, transform.position, adjustedRotation);
        Supernova fireExpell = explosion.GetComponent<Supernova>();
        if (isHeavy)
        {
            fireExpell.SetDamageStats(supernovaLightDamage, supernovaLightFire, supernovaLightPerSeconds);
        }
        else
        {
            fireExpell.SetDamageStats(supernovaHeavyDamage, supernovaHeavyFire, supernovaHeavyPerSeconds);
        }
        
        EnemyHealth enemyHealth = collision.gameObject.GetComponent<EnemyHealth>();
        if(enemyHealth != null){
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

        Destroy(gameObject);
    }
}
