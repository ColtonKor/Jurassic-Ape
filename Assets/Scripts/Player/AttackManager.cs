using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class AttackManager : MonoBehaviour
{
    private PlayerStateMachine playerStateMachine;
    public bool shieldActive;
    
    private PowerManager powerManager;
    private bool isDodging;
    private Vector3 direction;
    
    public List<GameObject> tools = new List<GameObject>();
    public List<GameObject> backTools = new List<GameObject>();
    public GameObject powerLocation;
    private Superpowers currentPower;
    private int currentIndex = 0;
    private Coroutine removeShieldCoroutine;
    private bool currentAxe;
    private bool currentSword;
    private bool currentFist;
    private bool idleShield;
    private bool isShooting;
    private bool isBlocking;
    private bool shiftedControls;
    private bool shiftedAim;
    [HideInInspector]public bool screamReady;
    private UIManager uiManager;
    private Weapon currentMelee;
    private PlayerHealth playerHealth;

    private bool paused;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerStateMachine = GetComponent<PlayerStateMachine>();
        uiManager = GetComponent<UIManager>();
        powerManager = GetComponent<PowerManager>();
        
        currentPower = powerManager.powers[currentIndex];
        currentMelee = tools[3].GetComponent<Weapon>();
        uiManager.AssignValues();
        uiManager.specialIndicatorParent.SetActive(false);
        
        playerStateMachine.PlayerInput.Player.Weapon.started += Weapon;
        playerStateMachine.PlayerInput.Player.Weapon.performed += Weapon;
        playerStateMachine.PlayerInput.Player.BlockShiftToSpecialAttacks.started += Block;
        playerStateMachine.PlayerInput.Player.BlockShiftToSpecialAttacks.performed += Block;
        playerStateMachine.PlayerInput.Player.BlockShiftToSpecialAttacks.canceled += Block;
        playerStateMachine.PlayerInput.Player.Aim.started += Aim;
        playerStateMachine.PlayerInput.Player.Aim.canceled += Aim;
        playerStateMachine.PlayerInput.Player.LightMeleeAttack.started += LightMeleeAttack;
        playerStateMachine.PlayerInput.Player.HeavyMeleeAttack.started += HeavyMeleeAttack;
        playerStateMachine.PlayerInput.Player.RangedAttack.started += RangedAttack;
        playerStateMachine.PlayerInput.Player.RangedAttack.canceled += RangedAttack;
        playerStateMachine.PlayerInput.Player.Powercodex.started += PowerCodex;
        playerStateMachine.PlayerInput.Player.Healing.started += Heal;
    }
    
    public void Heal(InputAction.CallbackContext context){
        if (playerHealth.currentHealSpells > 0 && playerHealth.currentHealth < playerHealth.maxHealth)
        {
            playerHealth.Heal();

        }
    }
    
    public void Weapon(InputAction.CallbackContext context)
    {
        float button = context.ReadValue<float>();
        if (button == -1)
        {
            if (!currentAxe)
            {
                if (currentSword) 
                { 
                    //Move Poleblade back to the hip/back
                    tools[1].SetActive(false);
                    backTools[1].SetActive(true);
                } 
                currentAxe = true;
                currentSword = false;
                currentFist = false;
                //Current weapon is Axe
                //Move the Axe to the hand
                tools[0].SetActive(true);
                currentMelee = tools[0].GetComponent<Weapon>();
                backTools[0].SetActive(false);
                tools[3].SetActive(false);
                uiManager.WeaponSpriteIndicatior();
                uiManager.specialIndicatorParent.SetActive(true);
                uiManager.AssignSpecialAttacks();
            }
            else
            {
                currentAxe = false;
                currentSword = false;
                currentFist = true;
                //Current weapon is Fists
                //Put Axe back on player
                tools[0].SetActive(false);
                backTools[0].SetActive(true);
                tools[3].SetActive(true);
                currentMelee = tools[3].GetComponent<Weapon>();
                uiManager.WeaponSpriteIndicatior();
                uiManager.specialIndicatorParent.SetActive(false);
            }
        }
        else if (button == 1)
        {
            if (!currentSword)
            {
                if (currentAxe)
                {
                    //Put Axe back on back/hip
                    tools[0].SetActive(false);
                    backTools[0].SetActive(true);
                }
                currentSword = true;
                currentAxe = false;
                currentFist = false;
                //Current weapon is Poleblade
                //Move the Poleblade to the hands
                tools[1].SetActive(true);
                currentMelee = tools[1].GetComponent<Weapon>();
                backTools[1].SetActive(false);
                tools[3].SetActive(false);
                uiManager.WeaponSpriteIndicatior();
                uiManager.specialIndicatorParent.SetActive(true);
                uiManager.AssignSpecialAttacks();
            }
            else
            {
                currentAxe = false;
                currentSword = false;
                currentFist = true;
                // currentMelee = arsenal[0];
                //Current weapon is Fists
                //Return the Poleblade to player
                tools[1].SetActive(false);
                backTools[1].SetActive(true);
                tools[3].SetActive(true);
                currentMelee = tools[3].GetComponent<Weapon>();
                uiManager.WeaponSpriteIndicatior();
                uiManager.specialIndicatorParent.SetActive(false);
            }
        }
    }
    
    public void HeavyMeleeAttack(InputAction.CallbackContext context){
        if(context.started){
            if(!shiftedControls && !shiftedAim){
                if (isDodging)
                {
                    return;
                }
                if(!isShooting){
                    currentMelee.isHeavy = false;
                }
            }
        }
    }
    
    public void LightMeleeAttack(InputAction.CallbackContext context){
        if(context.started){
            if(!shiftedControls){
                if (isDodging)
                {
                    return;
                }
                if(!isShooting){
                    currentMelee.isHeavy = false;
                }
            }
            else
            {
                
            }
        }
    }
    
    public void RangedAttack(InputAction.CallbackContext context){
        if(!shiftedControls){
            if (isDodging)
            {
                return;
            }
            if(isShooting){
                switch (currentPower.rangeType)
                {
                    case Superpowers.RangeType.heatVision:
                        if (context.started)
                        {
                            if(powerManager.currentVisionCapacity > 0){
                                Debug.Log("Heat Vision");
                                
                                powerManager.laser.gameObject.SetActive(true);
                                powerManager.depleteVision = true;
                                powerManager.rechargeVisionTimer = false;
                                powerManager.chargeVision = false;
                            }
                        } 
                        else if (context.canceled)
                        {
                            //Cancel the Laser Beams
                            if(!powerManager.rechargeVisionTimer && !powerManager.chargeVision)
                            {
                                powerManager.rechargeVisionTimer = true;
                                powerManager.depleteVision = false;
                                powerManager.laser.gameObject.SetActive(false);
                            }
                        }
                        
                        break;
                    case Superpowers.RangeType.sonicScream:
                        if (context.started)
                        {
                            if(powerManager.currentScreamCapacity < powerManager.maxScreamCapacity){
                                Debug.Log("Sonic Scream");
                                screamReady = true;
                                powerManager.depleteScream = false;
                                powerManager.chargeScream = true;
                                powerManager.rechargeScreamTimer = false;
                                powerManager.StartScreamCapacity();
                            } 
                        }
                        else if (context.canceled)
                        {
                            if (screamReady)
                            {
                                //Send the Sonic Scream
                                screamReady = false;
                                powerManager.chargeScream = false;
                                Superpowers sonicScream = Instantiate(currentPower, powerLocation.transform.position,
                                    playerStateMachine.Camera.transform.rotation);
                                sonicScream.direction = playerStateMachine.Camera.transform.forward;
                                sonicScream.currentCapacity = powerManager.currentScreamCapacity - powerManager.GetStartScreamCapacity();
                                powerManager.rechargeScreamTimer = true;
                            }
                        }
                        
                        break;
                    case Superpowers.RangeType.brainBlast:
                        if (context.started)
                        {
                            if(powerManager.currentBrainCapacity > 0){
                                Debug.Log("Brain Blast");
                                Superpowers projectile = Instantiate(currentPower, powerLocation.transform.position, playerStateMachine.Camera.transform.rotation);
                                projectile.direction = playerStateMachine.Camera.transform.forward;
                                powerManager.currentBrainCapacity--;
                                uiManager.TakePowerCharge();
                            }
                        }
                        break;
                }
            }
        }
    }
    
    public void Block(InputAction.CallbackContext context)
    {
        if (context.performed || context.started)
        {
            if (idleShield)
            {
                StopCoroutine(removeShieldCoroutine);
                idleShield = false;
            }
            tools[2].SetActive(true);
            isBlocking = true;
        } 
        else if (context.canceled)
        {
            removeShieldCoroutine = StartCoroutine(RemoveShield());
            isBlocking = false;
        }
    }
    
    public void Aim(InputAction.CallbackContext context){
        if(context.started || context.performed){
            if (isDodging)
            {
                return;
            }
    
            shiftedAim = true;
            isShooting = true;
            uiManager.ToggleCrosshair();
            
            playerStateMachine.normalCamera.SetActive(false);
            playerStateMachine.aimingCamera.SetActive(true);
            //Bring currentWeapon.gameObject to body Location
        } else if (context.canceled) {
            //Bring currentWeapon.gameObject to hand Location
            shiftedAim = false;
            isShooting = false;
            uiManager.ToggleCrosshair();
            
            playerStateMachine.normalCamera.SetActive(true);
            playerStateMachine.aimingCamera.SetActive(false);
        }
    }
    
    public void PowerCodex(InputAction.CallbackContext context)
    {
        if(context.started)
        {
            switch (currentPower.rangeType)
            {
                case Superpowers.RangeType.heatVision:
                    if(!powerManager.rechargeVisionTimer && !powerManager.chargeVision)
                    {
                        powerManager.rechargeVisionTimer = true;
                        powerManager.depleteVision = false;
                        powerManager.laser.gameObject.SetActive(false);
                    }

                    break;
                case Superpowers.RangeType.sonicScream:
                    if (screamReady)
                    {
                        //Send the Sonic Scream
                        screamReady = false;
                        powerManager.chargeScream = false;
                        Superpowers sonicScream = Instantiate(currentPower, powerLocation.transform.position,
                            playerStateMachine.Camera.transform.rotation);
                        sonicScream.direction = playerStateMachine.Camera.transform.forward;
                        sonicScream.currentCapacity = powerManager.currentScreamCapacity;
                        powerManager.rechargeScreamTimer = true;
                    }

                    break;
            }

            currentIndex = (currentIndex + 1) % powerManager.powers.Count;
            currentPower = powerManager.powers[currentIndex];
            uiManager.PowerSpriteIndicatior(currentIndex);
        }
    }
    
    
    public IEnumerator RemoveShield()
    {
        idleShield = true;
        yield return new WaitForSeconds(5f);
        tools[2].SetActive(false);
        idleShield = false;
    }
    
    public Superpowers CurrentPower { get { return currentPower; }  }
    public Weapon CurrentMelee { get { return currentMelee; } }
    public bool IsShooting { get { return isShooting; } }
    public bool IsBlocking { get { return isBlocking; } }
    public bool ShiftedAim { get { return shiftedAim; } }
}
