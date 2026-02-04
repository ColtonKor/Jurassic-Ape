using UnityEngine;

public class Laser : Power
{
    public float lightFireDamage;
    public float heavyFireDamage;
    public int fireTime;
    public GameObject superNova;
    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            return;
        }

        if (isHeavy)
        {
            GameObject supernova = Instantiate(superNova, transform.position, transform.rotation);
            Supernova explosion = supernova.GetComponent<Supernova>();
            explosion.SetDamageStats(heavyDamage, heavyStun, heavyFireDamage, fireTime);
        }
        EnemyHealth enemyHealth = other.GetComponent<EnemyHealth>();
        if(enemyHealth != null)
        {
            if (!isHeavy)
            {
                enemyHealth.TakeDamage(lightDamage);
                enemyHealth.TakeStun(lightStun);
                enemyHealth.TakeFireDamage(lightFireDamage, fireTime);
                
            }
            // else
            // {
            //     enemyHealth.TakeDamage(heavyDamage);
            //     enemyHealth.TakeStun(heavyStun);
            //     enemyHealth.TakeFireDamage(heavyFireDamage, fireTime);
            // }
        }
        Destroy(gameObject);
    }

    public void Update()
    {
        transform.Translate(direction * speed * Time.deltaTime, Space.World);
    }
}
