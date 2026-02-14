using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class OldAttackManager : MonoBehaviour
{
    private PlayerMovement playerMovement;
    private PlayerHealth playerHealth;
    public List<Power> abilities = new List<Power>();
    public List<Weapon> arsenal = new List<Weapon>();
    private Power currentItem;
    private Weapon currentMelee;
    public GameObject itemLocation;
    public GameObject directionObject;
    private Vector3 direction;
    public float cooldownTime = 1f;
    [HideInInspector]
    public bool shieldActive = false;
    [HideInInspector]
    public bool shooting = false;
    private GameObject currentWeapon;
    private int currentIndex = 0;
    private bool isRefill = false;
    public int healthMaxAmmo;
    public int healthCurrentAmmo;
    public int maxAmmo;
    public int currentAmmo;
    private bool shiftedControls = false;
    private Camera cam;
    private bool currentFists = false;
    private bool currentAxe = false;
    private bool currentPoleblade = false;
    private bool isChargingWeapon;
    public float chargingTimer = 2f;
    private float timer = 0;

    public GameObject normalCamera;
    public GameObject aimingCamera;

    void Start(){
        healthCurrentAmmo = healthMaxAmmo;
        currentAmmo = maxAmmo;
        playerMovement = GetComponent<PlayerMovement>();
        playerHealth = GetComponent<PlayerHealth>();
        currentItem = abilities[0];
        cam = Camera.main;
    }

    public void Block(InputAction.CallbackContext context){
        if(context.started || context.performed){
            if (playerMovement.isDodging)
            {
                return;
            }
            shiftedControls = true;
            shieldActive = true;
        } else if (context.canceled) {
            shiftedControls = false;
            shieldActive = false;
        }
    }

    public void Aim(InputAction.CallbackContext context){
        if(context.started || context.performed){
            if (playerMovement.isDodging)
            {
                return;
            }
            shieldActive = false;
            shooting = true;
            
            // normalCamera.SetActive(false);
            // aimingCamera.SetActive(true);
            //Bring currentWeapon.gameObject to body Location
        } else if (context.canceled) {
            //Bring currentWeapon.gameObject to hand Location
            shooting = false;
            // normalCamera.SetActive(true);
            // aimingCamera.SetActive(false);
        }
    }

    public void Weapon(InputAction.CallbackContext context)
    {
        if (context.started || context.performed)
        {
            float button = context.ReadValue<float>();
            if (button == -1)
            {
                if (!currentAxe)
                {
                    if (currentPoleblade)
                    {
                        //Move Poleblade back to the hip/back
                    }
                    currentAxe = true;
                    currentFists = false;
                    currentPoleblade = false;
                    currentMelee = arsenal[1];
                    Debug.Log("Current Weapon is " + currentMelee.name);
                    //Current weapon is Axe
                    //Move the Axe to the hand
                }
                else
                {
                    currentAxe = false;
                    currentPoleblade = false;
                    currentFists = true;
                    currentMelee = arsenal[0];
                    Debug.Log("Current Weapon is " + currentMelee.name);
                    //Current weapon is Fists
                    //Put Axe back on player
                }
            }
            else if (button == 1)
            {
                if (!currentPoleblade)
                {
                    if (currentAxe)
                    {
                        //Put Axe back on back/hip
                    }
                    currentPoleblade = true;
                    currentFists = false;
                    currentAxe = false;
                    currentMelee = arsenal[2];
                    Debug.Log("Current Weapon is " + currentMelee.name);
                    //Current weapon is Poleblade
                    //Move the Poleblade to the hands
                }
                else
                {
                    currentAxe = false;
                    currentPoleblade = false;
                    currentFists = true;
                    currentMelee = arsenal[0];
                    Debug.Log("Current Weapon is " + currentMelee.name);
                    //Current weapon is Fists
                    //Return the Poleblade to player
                }
            }
        }
    }

    public void SpecialAttackHeavy(InputAction.CallbackContext context){
        if(context.started){
            if (playerMovement.isDodging)
            {
                return;
            }
            if(shiftedControls){
                //Activate left Special Attack
                Debug.Log("Special Heavy Attack");
            }
        }
    }

    public void SpecialAttackLight(InputAction.CallbackContext context){
        if(context.started){
            if (playerMovement.isDodging)
            {
                return;
            }
            if(shiftedControls){
                //Activate right Special Attack
                Debug.Log("Special Light Attack");
            }
        }
    }

    public void Powercodex(InputAction.CallbackContext context){
        if(context.started){
            currentIndex = (currentIndex + 1) % abilities.Count;
            currentItem = abilities[currentIndex];
        }
    }

    public void ChargeWeapon(InputAction.CallbackContext context){
        if(context.started){
            if (playerMovement.isDodging)
            {
                return;
            }
            isChargingWeapon = true;
        } 
        else if (context.canceled)
        {
            isChargingWeapon = false;
        }
    }

    public void HeavyAttack(InputAction.CallbackContext context){
        if(context.started){
            if(!shiftedControls){
                if (playerMovement.isDodging)
                {
                    return;
                }
                if(shooting){
                    if(currentAmmo > 0){
                        Debug.Log("Heavy Power Attack");
                        Power projectile = Instantiate(currentItem, itemLocation.transform.position, cam.transform.rotation);
                        projectile.isHeavy = true;
                        projectile.direction = cam.transform.forward;
                        currentAmmo--;
                    }
                } else {
                    Debug.Log("Heavy Swing Attack");
                    playerMovement.WeaponRotate();
                    if (currentMelee.isCharged)
                    {
                        // Use the charged heavy attack  
                        currentMelee.isCharged = false;
                    }
                    currentMelee.isHeavy = true;
                }
            }
        }
    }

    public void LightAttack(InputAction.CallbackContext context){
        if(context.started){
            if(!shiftedControls){
                if (playerMovement.isDodging)
                {
                    return;
                }
                if(shooting){
                    if(currentAmmo > 0){
                        Debug.Log("Light Power Attack");
                        Power projectile = Instantiate(currentItem, itemLocation.transform.position, cam.transform.rotation);
                        projectile.isHeavy = false;
                        projectile.direction = cam.transform.forward;
                        currentAmmo--;
                    }
                } else {
                    Debug.Log("Light Swing Attack");
                    playerMovement.WeaponRotate();
                    if (currentMelee.isCharged)
                    {
                        // Use the charged light attack  
                        currentMelee.isCharged = false;
                    }
                    currentMelee.isHeavy = false;
                }
            }
        }
    }

    void Update(){
        if (isChargingWeapon)
        {
            timer += Time.deltaTime;
            if (timer >= chargingTimer)
            {
                currentMelee.isCharged = true;
                Debug.Log("Charged your current Weapon");
            }
        }
        else
        {
            timer = 0;
        }
        if(maxAmmo > currentAmmo && !isRefill){
            StartCoroutine(Refill());
        }
    }

    private IEnumerator Refill(){
        isRefill = true;
        yield return new WaitForSeconds(3f);
        Debug.Log("Refilled Fire Ammo: " + currentAmmo);
        isRefill = false;
        currentAmmo++;
    }
}
