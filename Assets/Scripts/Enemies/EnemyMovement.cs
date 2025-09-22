using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    public bool canBePulled;
    private bool isBeingPulled;
    public float speed;
    public float movementDurability = 1;
    private GameObject target;
    private Rigidbody rb;
    private EnemyHealth enemyHealth;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        enemyHealth = GetComponent<EnemyHealth>();
    }

    void Update()
    {
        if (!enemyHealth.isStunned)
        {
            
        }
        if (isBeingPulled)
        {
            PullEnemy();
        }
    }
    
    public void SetPullStatus(bool current)
    {
        if (canBePulled)
        {
            isBeingPulled = current;
            if (!current)
            {
                StopPull();
            }
        }
    }
    private void StopPull()
    {
        isBeingPulled = false;
        rb.useGravity = true;
        rb.linearVelocity = Vector3.zero;
    }

    public void SetTarget(GameObject target){
        this.target = target;
    }

    public void PullEnemy(){
        rb.useGravity = false;

        if (target == null)
        {
            StopPull();
            return;
        }

        Vector3 direction = (target.transform.position - transform.position).normalized;
        float distance = Vector3.Distance(transform.position, target.transform.position);

        if (distance > 0.1f){
            rb.linearVelocity = direction * speed;
        } else {
            StopPull();
        }
    }
    
    public void Knockback(Transform knockback)
    {
        Vector3 direction = (transform.position - knockback.position).normalized;
        direction = new Vector3(direction.x, 0, direction.z);
        rb.AddForce(direction, ForceMode.Impulse);
    }
}
