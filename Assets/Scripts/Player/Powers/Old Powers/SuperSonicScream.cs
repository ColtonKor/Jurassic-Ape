using UnityEngine;

public class SuperSonicScream : Power
{
    public float duration = 3f;
    public void Start()
    {
        Destroy(gameObject, duration);
    }
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
            }
            else
            {
                enemyHealth.TakeDamage(lightDamage);
                enemyHealth.TakeStun(lightStun);
            }
        }
    }

    public void Update()
    {
        transform.Translate(direction * speed * Time.deltaTime, Space.World);
        if (isHeavy)
        {
            transform.localScale = new Vector3(transform.localScale.x + .05f, transform.localScale.y + .05f, 1);
        }
    }
}
