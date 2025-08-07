using UnityEngine;

public class HeatVision : Power
{
    public float lightFireDamage;
    public float heavyFireDamage;
    public int fireTime;
    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            return;
        }
        EnemyHealth enemyHealth = other.GetComponent<EnemyHealth>();
        if(enemyHealth != null)
        {
            if (isHeavy)
            {
                enemyHealth.TakeDamage(heavyDamage);
                enemyHealth.TakeStun(heavyStun);
                enemyHealth.TakeFireDamage(heavyFireDamage, fireTime);
            }
            else
            {
                enemyHealth.TakeDamage(lightDamage);
                enemyHealth.TakeStun(lightStun);
                enemyHealth.TakeFireDamage(lightFireDamage, fireTime);
            }
        }
        Destroy(gameObject);
    }

    public void Update()
    {
        transform.Translate(direction * speed * Time.deltaTime, Space.World);
    }
}
