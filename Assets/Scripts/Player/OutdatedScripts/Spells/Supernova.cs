using System;
using UnityEngine;

public class Supernova : MonoBehaviour
{

    private float damage;

    private float fireDamage;
    private int fireTime;
    
    public float wallDuration;
    private Rigidbody rb;


    public void SetDamageStats(float damage, float fireDamage, int fireTime)
    {
        this.damage = damage;
        this.fireDamage = fireDamage;
        this.fireTime = fireTime;
    }

    void Start()
    {
        Destroy(gameObject, wallDuration);
    }

    public void OnTriggerEnter(Collider other)
    {
        EnemyMovement enemyMovement = other.gameObject.GetComponent<EnemyMovement>();
        if(enemyMovement != null)
        {
            EnemyHealth enemyHealth = other.GetComponent<EnemyHealth>();
            enemyHealth.TakeDamage(damage);
            enemyHealth.TakeFireDamage(fireDamage, fireTime);
            enemyMovement.Knockback(gameObject.transform);
        }
    }
}
