using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public float maxHealth = 100f;
    public float maxStun = 50f;
    public float currentStun = 0f;
    public float currentHealth = 0;
    public float stunTime;
    private bool flamed = false;
    private int secondsForFire;
    private GameObject blackhole;
    private float maxBlackholeDamage;
    private float maxBlackholeStun;
    private float currentBlackholeDamage;
    private float currentBlackholeStun;
    private float elapsedTime = 0f;
    public bool isStunned;
    private Animator animator;

    void Start(){
        currentHealth = maxHealth;
        animator = GetComponent<Animator>();
    }

    void Update(){
        if(blackhole != null){
            elapsedTime += Time.deltaTime;
        
            float distance = Vector3.Distance(transform.position, blackhole.transform.position);
            
            float closestDistance = 0.25f;
            float farthestDistance = 3.5f;
        
            float proximityFactor = Mathf.Clamp01((farthestDistance - distance) / (farthestDistance - closestDistance));
            currentBlackholeDamage = maxBlackholeDamage * proximityFactor;
            currentBlackholeStun = maxBlackholeStun * proximityFactor;
        
            if (elapsedTime >= 1f){
                TakeDamage(currentBlackholeDamage);
                TakeStun(currentBlackholeStun);
                elapsedTime -= 1f;
            }
        }
    }
    

    public void TakeDamage(float damage){
        currentHealth -= damage;
        if(currentHealth <= 0){
            OnDefeat();
        }
    }

    public void TakeStun(float damage){
        currentStun += damage;
        if(currentStun >= maxStun){
            StartCoroutine(StunEnemy());
        }
    }

    public void TakeFireDamage(float damage, int seconds){
        secondsForFire = seconds;
        if(!flamed)
        {
            StartCoroutine(FireDamage(damage));
        }
    }

    private IEnumerator FireDamage(float damage){
        flamed = true;
        currentHealth -= damage;
        if(currentHealth <= 0)
        {
            OnDefeat();
        }
        yield return new WaitForSeconds(1);
        if(secondsForFire > 0){
            secondsForFire--;
            StartCoroutine(FireDamage(damage));
        } else {
            secondsForFire = 5;
            flamed = false;
        }
    }

    private IEnumerator StunEnemy(){
        //Turn AI state to stun
        isStunned = true;
        yield return new WaitForSeconds(stunTime);
        //Turn AI state to normal;
        isStunned = false;
    }

    private void OnDefeat()
    {
        StartCoroutine(Dying());
    }

    public void SetCenterDamageMath(GameObject center){
        blackhole = center;
    }
    
    public void SetBlackholeStun(float stun){
        maxBlackholeStun = stun;
    }
    
    public void SetBlackholeDamage(float damage){
        maxBlackholeDamage = damage;
    }

    private IEnumerator Dying()
    {
        animator.SetBool("Died", true);
        yield return new WaitForSeconds(3f);
        Destroy(gameObject);
    }
}
