using UnityEngine;
using System.Collections.Generic;


public class BrainBolt : Power
{
    
    public int maxBounces = 3;
    private int currentBounces = 0;
    private bool startBouncing = false;
    [SerializeField] public float enemyRadius;
    [SerializeField] public LayerMask enemyLayer;
    private GameObject currentEnemy;
    private GameObject nextEnemy;
    private bool soloEnemy;
    private float soloEnemyTimer = 0f;
    private float enemyTimer = 0f;
    private EnemyHealth enemyHealth;
    private bool stopMoving;

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            return;
        } 
        enemyHealth = other.GetComponent<EnemyHealth>();
        if(enemyHealth != null)
        {
            currentEnemy = other.gameObject;
            HitFunction();
        }
        else
        {
            if (currentBounces == 0)
            {
                Destroy(gameObject);
            }
        }
    }

    public void Update()
    {
        if (startBouncing)
        {
            stopMoving = true;
            enemyTimer += Time.deltaTime;
            if (enemyTimer >= 1f)
            {
                startBouncing = false;
                CheckForEnemy();
                enemyTimer = 0f;
                stopMoving = false;
            }
        }

        if (soloEnemy)
        {
            soloEnemyTimer += Time.deltaTime;
            if (soloEnemyTimer >= 1f)
            {
                HitFunction();
                soloEnemyTimer = 0f;
            }
        }

        if (currentBounces > 0 && nextEnemy != null)
        {
            direction = (nextEnemy.transform.position - transform.position).normalized;
        }

        if (!stopMoving && !soloEnemy)
        {
            transform.Translate(direction * speed * Time.deltaTime, Space.World);
        }
        
    }
    
    void CheckForEnemy(){
        Collider[] hits = Physics.OverlapSphere(transform.position, enemyRadius, enemyLayer);
        if (hits.Length > 0){
            nextEnemy = hits[Random.Range(0, hits.Length)].gameObject;
            if (currentEnemy == nextEnemy)
            {
                soloEnemy = true;
            }
            else 
            {
                soloEnemy = false;
            }
            
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void HitFunction()
    {
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
            
            if (currentBounces >= maxBounces)
            {
                Destroy(gameObject);
            }
            else
            {
                currentBounces++;
                startBouncing = true;
            }
        }
    }
}
