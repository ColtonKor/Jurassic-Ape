using UnityEngine;

public class SonicScream : Superpowers
{
    public float duration = 3f;
    public float speed;
    public float chargePower = 2f;
    [SerializeField] private Vector2 maxSize;
    private float chargePercent;
    private float scaledCharge;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        chargePercent = Mathf.Clamp01(currentCapacity / maxCapacity);
        scaledCharge = Mathf.Pow(chargePercent, chargePower);
        
        Vector3 minSize = transform.localScale;
        Vector3 maxSize = new Vector3(3f, 3f, 0.1f);
        transform.localScale = Vector3.Lerp(minSize, maxSize, chargePercent);

        Destroy(gameObject, duration);
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(direction * speed * Time.deltaTime, Space.World);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            return;
        }
        EnemyHealth enemyHealth = other.GetComponent<EnemyHealth>();
        if(enemyHealth != null)
        {
            enemyHealth.TakeDamage(damage * (1f + scaledCharge));
            enemyHealth.TakeStun(stun * (1f + scaledCharge));
        }
    }
}
