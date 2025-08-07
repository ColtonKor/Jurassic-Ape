using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlackholeSpell : Spells
{
    public GameObject blackholePrefab;
    private bool hitAlready = false;
    void OnTriggerEnter(Collider collision){
        if(collision.isTrigger){
            return;
        }
        if(!hitAlready){
            hitAlready = true;
            GameObject explosion = Instantiate(blackholePrefab, transform.position, Quaternion.identity);
            Blackhole gravityPull = explosion.GetComponent<Blackhole>();
            if (isHeavy)
            {
                gravityPull.SetStats(heavyDamage, heavyStun);
            }
            else
            {
                gravityPull.SetStats(lightDamage, lightStun);
            }
            
            Destroy(gameObject);
        }
    }
}
