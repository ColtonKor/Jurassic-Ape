using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SonicboomSpell : Spells
{
    public GameObject sonicboomPrefab;
    public float explosionLightStun;
    public float explosionHeavyStun;
    //Deals massive stun to hit enemy but creates explosion that slows enemies and deal little stun damage.
    void OnTriggerEnter(Collider collision){
        if(collision.isTrigger){
            return;
        }
        GameObject explode = Instantiate(sonicboomPrefab, transform.position, Quaternion.identity);
        Sonicboom explosionEffect = explode.GetComponent<Sonicboom>();
        if (explosionEffect != null)
        {
            if (isHeavy)
            {
                explosionEffect.SetStunDamage(explosionHeavyStun);
            }
            else
            {
                explosionEffect.SetStunDamage(explosionLightStun);
            }
            
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
