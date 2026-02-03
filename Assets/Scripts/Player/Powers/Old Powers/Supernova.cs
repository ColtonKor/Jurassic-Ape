using System;
using UnityEngine;

public class Supernova : MonoBehaviour
{

    private float damage;
    private float stun;

    private float fireDamage;
    private int fireTime;
    
    public float wallDuration;
    private Rigidbody rb;


    public void SetDamageStats(float damage, float stun, float fireDamage, int fireTime)
    {
        this.damage = damage;
        this.stun = stun;
        this.fireDamage = fireDamage;
        this.fireTime = fireTime;
    }

    void Start()
    {
        Destroy(gameObject, wallDuration);
    }

    public void OnTriggerEnter(Collider other)
    {
        EnemyHealth enemyHealth = other.GetComponent<EnemyHealth>();
        if(enemyHealth != null)
        {
            enemyHealth.TakeDamage(damage);
            enemyHealth.TakeStun(stun);
            enemyHealth.TakeFireDamage(fireDamage, fireTime);
        }
    }
}
