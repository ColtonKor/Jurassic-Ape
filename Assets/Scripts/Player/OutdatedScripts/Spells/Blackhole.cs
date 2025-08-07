using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blackhole : MonoBehaviour
{
    private float maxDamagePerSecond;
    private float maxStunPerSecond;
    void Start(){
        Destroy(gameObject, 7.5f);   
    }

    void OnTriggerEnter(Collider collision){
        EnemyHealth enemyHealth = collision.gameObject.GetComponent<EnemyHealth>();
        if(enemyHealth != null){
            EnemyMovement enemyMovement = collision.gameObject.GetComponent<EnemyMovement>();
            if(enemyMovement != null){
                enemyMovement.SetPullStatus(true);
                enemyMovement.SetTarget(gameObject);
            }
            enemyHealth.SetCenterDamageMath(gameObject);
            enemyHealth.SetBlackholeDamage(maxDamagePerSecond);
            enemyHealth.SetBlackholeStun(maxStunPerSecond);
        }
    }

    void OnTriggerExit(Collider collision){
        EnemyMovement enemyMovement = collision.gameObject.GetComponent<EnemyMovement>();
        EnemyHealth enemyHealth = collision.gameObject.GetComponent<EnemyHealth>();
        if(enemyMovement != null){
            enemyHealth.SetCenterDamageMath(null);
            enemyHealth.SetBlackholeDamage(0);
            enemyHealth.SetBlackholeStun(0);
            enemyMovement.SetPullStatus(false);
            enemyMovement.SetTarget(null);
        }
    }

    public void SetStats(float damage, float stun){
        maxDamagePerSecond = damage;
        maxStunPerSecond = stun;
    }
}
