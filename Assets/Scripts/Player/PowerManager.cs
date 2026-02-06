using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerManager : MonoBehaviour
{
    private UIManager uIManager;
    private PlayerStateMachine playerStateMachine;
    public List<Superpowers> powers = new List<Superpowers>();
    
    public float maxScreamCapacity;
    public float currentScreamCapacity;
    
    public GameObject laser;
    public float maxVisionCapacity;
    public float currentVisionCapacity;

    public int maxBrainCapacity = 4;
    [HideInInspector]public int currentBrainCapacity;
    private bool isRefill;
    
    
    private float chargeAmount = 10f;
    private float chargeInterval = 1f;
    private float chargeTimer;

    private float rechargeTimer;
    private float rechargeInterval = 2f;

    [HideInInspector] public bool chargeScream;
    [HideInInspector] public bool depleteScream;
    [HideInInspector] public bool chargeVision;
    [HideInInspector] public bool depleteVision;
    [HideInInspector] public bool rechargeScreamTimer;
    [HideInInspector] public bool rechargeVisionTimer;
    private HeatVision heatVision;

    void Start()
    {
        uIManager = GetComponent<UIManager>();
        playerStateMachine = GetComponent<PlayerStateMachine>();
        heatVision = powers[0].GetComponent<HeatVision>();
        SetPowerCapacity();
    }

    void Update()
    {
        if (chargeScream)
        {
            ChargeScream();
        } 
        else if (depleteScream)
        {
            DepleteScream();
        }

        if (depleteVision)
        {
            DepleteVision();
        }
        else if (chargeVision)
        {
            ChargeVision();
        }
        
        if(rechargeScreamTimer)
        {
            rechargeTimer += Time.deltaTime;
            if (rechargeTimer < rechargeInterval)
                return;
            rechargeScreamTimer = false;
            rechargeTimer = 0f;
            depleteScream = true;
        }
        
        if(rechargeVisionTimer)
        {
            rechargeTimer += Time.deltaTime;
            if (rechargeTimer < rechargeInterval)
                return;
            rechargeVisionTimer = false;
            rechargeTimer = 0f;
            chargeVision = true;
        }
        
        if(maxBrainCapacity > currentBrainCapacity && !isRefill){
            StartCoroutine(Refill());
        }
    }
    
    
    public void SetPowerCapacity()
    {
        uIManager.SetMaxPowers(maxVisionCapacity, maxScreamCapacity);
        uIManager.ChangeScream(currentScreamCapacity);
        uIManager.ChangeVision(currentVisionCapacity);
        powers[0].maxCapacity = maxVisionCapacity;
        powers[1].maxCapacity = maxScreamCapacity;
        currentBrainCapacity = maxBrainCapacity;
    }
    
    
    private void ChargeScream()
    {
        chargeTimer += Time.deltaTime;
        if (chargeTimer < chargeInterval)
            return;

        
        chargeTimer = 0f;

        currentScreamCapacity += chargeAmount;
        currentScreamCapacity = Mathf.Clamp(
            currentScreamCapacity,
            0f,
            maxScreamCapacity
        );

        uIManager.ChangeScream(currentScreamCapacity);
        if (currentScreamCapacity >= maxScreamCapacity)
        {
            chargeScream = false;
            depleteScream = false;
        }
    }
    
    private void DepleteScream()
    {
        powers[0].direction = playerStateMachine.Camera.transform.forward;
        chargeTimer += Time.deltaTime;
        if (chargeTimer < chargeInterval)
            return;

        
        chargeTimer = 0f;

        currentScreamCapacity -= chargeAmount;
        currentScreamCapacity = Mathf.Clamp(
            currentScreamCapacity,
            0f,
            maxScreamCapacity
        );

        uIManager.ChangeScream(currentScreamCapacity);
        if (currentScreamCapacity <= 0f)
        {
            chargeScream = false;
            depleteScream = false;
        }
    }

    private void DepleteVision()
    {
        heatVision.Propagate(laser.transform.position, playerStateMachine.Camera.transform.forward);
        chargeTimer += Time.deltaTime;
        if (chargeTimer < chargeInterval)
            return;

        
        chargeTimer = 0f;

        currentVisionCapacity -= chargeAmount;
        currentVisionCapacity = Mathf.Clamp(
            currentVisionCapacity,
            0f,
            maxVisionCapacity
        );

        uIManager.ChangeVision(currentVisionCapacity);
        if (currentVisionCapacity <= 0f)
        {
            rechargeVisionTimer = true;
            chargeVision = false;
            depleteVision = false;
            laser.gameObject.SetActive(false);
        }
    }
    
    private void ChargeVision()
    {
        chargeTimer += Time.deltaTime;
        if (chargeTimer < chargeInterval)
            return;

        
        chargeTimer = 0f;

        currentVisionCapacity += chargeAmount;
        currentVisionCapacity = Mathf.Clamp(
            currentVisionCapacity,
            0f,
            maxVisionCapacity
        );

        uIManager.ChangeVision(currentVisionCapacity);
        if (currentVisionCapacity >= maxVisionCapacity)
        {
            chargeVision = false;
            depleteVision = false;
        }
    }
    
    private IEnumerator Refill(){
        isRefill = true;
        yield return new WaitForSeconds(3f);
        // Debug.Log("Refilled Fire Ammo: " + currentAmmo);
        isRefill = false;
        currentBrainCapacity++;
        uIManager.AddPowerCharge();
    }
    
    public void CancelAllPowers()
    {
        // --- Sonic Scream ---
        chargeScream = false;
        depleteScream = false;
        rechargeScreamTimer = false;

        // --- Heat Vision ---
        chargeVision = false;
        depleteVision = false;
        rechargeVisionTimer = false;

        if (laser != null)
            laser.SetActive(false);

        // --- Shared timers ---
        chargeTimer = 0f;
        rechargeTimer = 0f;
    }
}
