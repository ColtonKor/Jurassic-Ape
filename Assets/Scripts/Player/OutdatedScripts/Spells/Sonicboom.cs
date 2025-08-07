using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sonicboom : MonoBehaviour
{
    public float stunDamage;
    public float duration = .25f;
    void Start(){
        Destroy(gameObject, duration);
    }

    private void OnTriggerEnter(Collider other){
        EnemyHealth enemyHealth = other.gameObject.GetComponent<EnemyHealth>();
        if(enemyHealth != null){
            enemyHealth.TakeStun(stunDamage);
        }
    }

    public void SetStunDamage(float stun){
        stunDamage = stun;
    }
}
