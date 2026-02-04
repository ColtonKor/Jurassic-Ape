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
    
    public float maxVisionCapacity;
    public float currentVisionCapacity;
    
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

    void Start()
    {
        uIManager = GetComponent<UIManager>();
        playerStateMachine = GetComponent<PlayerStateMachine>();
        SetPowerCapacity();
    }

    void Update()
    {
        if (chargeScream)
        {
            ChargeScream();
        } 
        else if (depleteVision)
        {
            DepleteVision();
        }

        if (depleteScream)
        {
            DepleteScream();
        }

        if (chargeVision)
        {
            
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
    }
    
    
    public void SetPowerCapacity()
    {
        uIManager.SetMaxPowers(powers[0].maxCapacity);
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
        if (currentScreamCapacity >= maxScreamCapacity)
        {
            chargeScream = false;
            depleteScream = false;
        }
    }

    private void DepleteVision()
    {
        currentVisionCapacity -= 10;
    }
    
    private void ChargeVision()
    {
        currentVisionCapacity -= 10;
    }
}
