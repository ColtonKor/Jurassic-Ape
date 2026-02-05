using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerStateMachine : MonoBehaviour
{
    private PlayerControls playerInput;
    private CharacterController characterController;
    private Animator animator;
    private Camera cam;
    public GameObject raptor;
    private Animator raptorAnimator;
    private PowerManager powerManager;

    private int isWalkingHash;
    private int isRunningHash;
    private int isJumpingHash;
    private int isDoubleJumpingHash;
    private int isFallingHash;
    private int isGlidingHash;
    private int isGroundedHash;
    private int isDodgeHash;
    private int isRidingHash;
    private int isGeyserGlidingHash;
    private int isSwimmingHash;

    private bool isJumpingAnimating;

    private Vector2 currentMovementInput;
    private Vector3 currentMovement;
    private Vector3 appliedMovement;

    private bool isMovementPressed;
    private bool isDodgePressed;
    private bool isRunPressed;
    private bool isJumpPressed;
    private bool isAirJumpPressed;
    private bool isGlidePressed;
    private bool isRidePressed;
    private bool requireNewJumpPress;
    private bool requireNewDodgePress;
    private bool isDodging;
    private bool isInGeyser;
    private bool isInWater;
    private bool isFloating;
    private bool raptorWaterDetection;

    private float speed = 3f;
    private float runMultiplier = 2f;
    private float rollSpeed = 20f;
    private float rollSpeedTemp;
    private float rollMinimum = 5f;
    private float raptorSpeed = 1f;
    private float rotationFactorPerFrame = 15f;
    private Vector3 direction;
    private Vector3 rollDirection;

    private float gravity = -9.8f;
    private float glideGravity = -2f;
    private float geyserLiftForce;

    private bool isJumping;
    float initialJumpVelocity;
    private float maxJumpHeight = 3f;
    private float maxJumpTime = 1f;

    public int maxNumberOfJumps = 1;
    private int currentNumberOfJumps;
    public int maxNumberOfRaptorJumps = 1;
    private int currentNumberOfRaptorJumps;
    
    PlayerBaseState currentState;
    private PlayerStateFactory states;
    
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
    private bool shooting;
    private bool shiftedControls;
    private bool shiftedAim;
    [HideInInspector]public bool screamReady;
    private UIManager uiManager;
    private Weapon currentMelee;
    private PlayerHealth playerHealth;
    
    
    public GameObject normalCamera;
    public GameObject aimingCamera;

    void Awake()
    {
        playerInput = new PlayerControls();
        characterController = GetComponent<CharacterController>();
        uiManager = GetComponent<UIManager>();
        animator = GetComponent<Animator>();
        playerHealth = GetComponent<PlayerHealth>();
        raptorAnimator = raptor.GetComponent<Animator>();
        powerManager = GetComponent<PowerManager>();
        cam = Camera.main;
        
        
        currentPower = powerManager.powers[currentIndex];
        currentMelee = tools[3].GetComponent<Weapon>();
        
        isWalkingHash = Animator.StringToHash("isWalking");
        isRunningHash = Animator.StringToHash("isRunning");
        isJumpingHash = Animator.StringToHash("isJumping");
        isDoubleJumpingHash = Animator.StringToHash("isDoubleJumping");
        isFallingHash = Animator.StringToHash("isFalling");
        isGlidingHash = Animator.StringToHash("isGliding");
        isGroundedHash = Animator.StringToHash("isGrounded");
        isDodgeHash = Animator.StringToHash("isDodge");
        isRidingHash = Animator.StringToHash("isRiding");
        isGeyserGlidingHash = Animator.StringToHash("isGeyserGliding");
        isSwimmingHash = Animator.StringToHash("isSwimming");
        
        animator.SetBool(isGroundedHash, true);

        states = new PlayerStateFactory(this);
        currentState = states.Grounded();
        currentState.EnterState();

        playerInput.Player.Move.performed += Move;
        playerInput.Player.Move.started += Move;
        playerInput.Player.Move.canceled += Move;
        playerInput.Player.Sprint.started += Sprint;
        playerInput.Player.Jump.started += Jump;
        playerInput.Player.Jump.canceled += Jump;
        playerInput.Player.Glide.started += Glide;
        playerInput.Player.Dodge.started += Dodge;
        playerInput.Player.Dodge.canceled += Dodge;
        playerInput.Player.Weapon.started += Weapon;
        playerInput.Player.Weapon.performed += Weapon;
        playerInput.Player.BlockShiftToSpecialAttacks.started += Block;
        playerInput.Player.BlockShiftToSpecialAttacks.performed += Block;
        playerInput.Player.BlockShiftToSpecialAttacks.canceled += Block;
        playerInput.Player.Powercodex.started += PowerCodex;
        playerInput.Player.Aim.started += Aim;
        playerInput.Player.Aim.canceled += Aim;
        playerInput.Player.LightMeleeAttack.started += LightMeleeAttack;
        playerInput.Player.HeavyMeleeAttack.started += HeavyMeleeAttack;
        playerInput.Player.RangedAttack.started += RangedAttack;
        playerInput.Player.RangedAttack.canceled += RangedAttack;
        playerInput.Player.Healing.started += Heal;
        playerInput.Player.CallMount.started += CallMount;

        SetupJumpVariables();
    }

    void SetupJumpVariables()
    {
        float timeToApex = maxJumpTime / 2;
        gravity = (-2 * maxJumpHeight) / Mathf.Pow(timeToApex, 2);
        initialJumpVelocity = (2 * maxJumpHeight) / timeToApex;
    }

    public void Move(InputAction.CallbackContext context)
    {
        currentMovementInput = context.ReadValue<Vector2>();
        currentMovement.x = currentMovementInput.x;
        currentMovement.z = currentMovementInput.y;
        isMovementPressed = currentMovementInput.x != 0 || currentMovementInput.y != 0;
    }

    public void Sprint(InputAction.CallbackContext context)
    {
        isRunPressed = !isRunPressed;
    }
    
    public void Glide(InputAction.CallbackContext context)
    {
        if (!characterController.isGrounded && !isRidePressed)
        {
            isGlidePressed = !isGlidePressed;
        }
    }
    
    public void Dodge(InputAction.CallbackContext context)
    {
        isDodgePressed = context.ReadValueAsButton();
        requireNewDodgePress = false;
    }

    public void Jump(InputAction.CallbackContext context)
    {
        isJumpPressed = context.ReadValueAsButton();
        requireNewJumpPress = false;
        if (!characterController.isGrounded)
        {
            isAirJumpPressed = context.ReadValueAsButton();
        }
    }
    
    public void Heal(InputAction.CallbackContext context){
        if(context.started){
            if(playerHealth.currentHealSpells > 0 && playerHealth.currentHealth < playerHealth.maxHealth){
                playerHealth.Heal();
            }
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
                // currentMelee = arsenal[1];
                Debug.Log("Current Weapon is Axe");
                uiManager.WeaponSpriteIndicatior(1);
                //Current weapon is Axe
                //Move the Axe to the hand
                tools[0].SetActive(true);
                currentMelee = tools[0].GetComponent<Weapon>();
                backTools[0].SetActive(false);
                tools[3].SetActive(false);
            }
            else
            {
                currentAxe = false;
                currentSword = false;
                currentFist = true;
                // currentMelee = arsenal[0];
                Debug.Log("Current Weapon is Fist");
                uiManager.WeaponSpriteIndicatior(0);
                //Current weapon is Fists
                //Put Axe back on player
                tools[0].SetActive(false);
                backTools[0].SetActive(true);
                tools[3].SetActive(true);
                currentMelee = tools[3].GetComponent<Weapon>();
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
                // currentMelee = arsenal[2];
                Debug.Log("Current Weapon is Sword");
                uiManager.WeaponSpriteIndicatior(2);
                //Current weapon is Poleblade
                //Move the Poleblade to the hands
                tools[1].SetActive(true);
                currentMelee = tools[1].GetComponent<Weapon>();
                backTools[1].SetActive(false);
                tools[3].SetActive(false);
            }
            else
            {
                currentAxe = false;
                currentSword = false;
                currentFist = true;
                // currentMelee = arsenal[0];
                Debug.Log("Current Weapon is Fist");
                uiManager.WeaponSpriteIndicatior(0);
                //Current weapon is Fists
                //Return the Poleblade to player
                tools[1].SetActive(false);
                backTools[1].SetActive(true);
                tools[3].SetActive(true);
                currentMelee = tools[3].GetComponent<Weapon>();
            }
        }
    }
    
    public void HeavyMeleeAttack(InputAction.CallbackContext context){
        if(context.started){
            if(!shiftedControls && !shiftedAim){
                if (isDodgePressed)
                {
                    return;
                }
                if(!shooting){
                    currentMelee.isHeavy = false;
                }
            }
        }
    }

    public void LightMeleeAttack(InputAction.CallbackContext context){
        if(context.started){
            if(!shiftedControls){
                if (isDodgePressed)
                {
                    return;
                }
                if(!shooting){
                    currentMelee.isHeavy = false;
                }
            }
        }
    }
    
    public void RangedAttack(InputAction.CallbackContext context){
        if(!shiftedControls){
            if (isDodgePressed)
            {
                return;
            }
            if(shooting){
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
                                    cam.transform.rotation);
                                sonicScream.direction = cam.transform.forward;
                                sonicScream.currentCapacity = powerManager.currentScreamCapacity;
                                powerManager.rechargeScreamTimer = true;
                            }
                        }
                        
                        break;
                    case Superpowers.RangeType.brainBlast:
                        if (context.started)
                        {
                            if(powerManager.currentBrainCapacity > 0){
                                Debug.Log("Brain Blast");
                                Superpowers projectile = Instantiate(currentPower, powerLocation.transform.position, cam.transform.rotation);
                                projectile.direction = cam.transform.forward;
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
        } 
        else if (context.canceled)
        {
            removeShieldCoroutine = StartCoroutine(RemoveShield());
        }
    }
    
    public void Aim(InputAction.CallbackContext context){
        if(context.started || context.performed){
            if (isDodgePressed)
            {
                return;
            }

            shiftedAim = true;
            shooting = true;
            uiManager.ToggleCrosshair();
            
            normalCamera.SetActive(false);
            aimingCamera.SetActive(true);
            //Bring currentWeapon.gameObject to body Location
        } else if (context.canceled) {
            //Bring currentWeapon.gameObject to hand Location
            shiftedAim = false;
            shooting = false;
            uiManager.ToggleCrosshair();
            
            normalCamera.SetActive(true);
            aimingCamera.SetActive(false);
        }
    }

    public void PowerCodex(InputAction.CallbackContext context)
    {
        if(context.started)
        {
            currentIndex = (currentIndex + 1) % powerManager.powers.Count;
            currentPower = powerManager.powers[currentIndex];
            uiManager.PowerSpriteIndicatior(currentIndex);
        }
    }

    public void CallMount(InputAction.CallbackContext context)
    {
        if (shiftedAim)
        {
            if(characterController.isGrounded || isRidePressed)
            {
                isRidePressed = !isRidePressed;
                HandleMount();
            }
        }
    }

    void Start()
    {
        characterController.Move(appliedMovement * Time.deltaTime);
    }

    void Update()
    {
        HandleRotation();
        currentState.UpdateStates();
        characterController.Move(appliedMovement * Time.deltaTime);
        
    }

    void HandleRotation()
    {
        Vector3 positionToLookAt;
        positionToLookAt.x = currentMovement.x;
        positionToLookAt.y = 0f;
        positionToLookAt.z = currentMovement.z;
        Quaternion currentRotation = transform.rotation;
        
        if (isMovementPressed || shooting)
        {
            Quaternion targetRotation = Quaternion.LookRotation(currentMovement);
            direction = Quaternion.Euler(0.0f, cam.transform.eulerAngles.y, 0.0f) * positionToLookAt;
            if (isRidePressed)
            {
                targetRotation = Quaternion.LookRotation(direction);
            }
            else if (!isRidePressed)
            {
                Vector3 cameraForward = cam.transform.forward;
                cameraForward.y = 0;
                cameraForward.Normalize();
                targetRotation = Quaternion.LookRotation(cameraForward, Vector3.up);
            }
            else if (isInWater && !isFloating)
            {
                Vector3 cameraForward = cam.transform.forward;
                cameraForward.Normalize();
                targetRotation = Quaternion.LookRotation(cameraForward, Vector3.up);
            }
            transform.rotation =
                Quaternion.Slerp(currentRotation, targetRotation, rotationFactorPerFrame * Time.deltaTime);
        }
    }

    void HandleMount()
    {
        if (isRidePressed)
        {
            raptor.SetActive(true);
            raptorSpeed = 2f;
            transform.position += new Vector3(0, 1.16f, 0);
            characterController.center = new Vector3(0, 0.3f, 0);
            characterController.height = 3;
        }
        else
        {
            characterController.center = new Vector3(0, 1, 0);
            characterController.height = 2;
            raptorSpeed = 1f;
            raptor.SetActive(false);
            animator.SetBool(isRidingHash, false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Geyser"))
        {
            isInGeyser = true;
            geyserLiftForce = other.GetComponent<Geyser>().liftForce;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Geyser"))
        {
            isInGeyser = false;
            geyserLiftForce = 0;
        }
    }

    public IEnumerator RemoveShield()
    {
        idleShield = true;
        yield return new WaitForSeconds(5f);
        tools[2].SetActive(false);
        idleShield = false;
    }

    void OnEnable()
    {
        playerInput.Player.Enable();
    }

    private void OnDisable()
    {
        playerInput.Player.Disable();
    }
    
    public PlayerBaseState CurrentState{ get { return currentState; } set { currentState = value; } }
    public bool IsJumpPressed{get { return isJumpPressed; }}
    public bool IsAirJumpPressed{get { return isAirJumpPressed; } set { isAirJumpPressed = value; } }
    public bool IsRunPressed {get { return isRunPressed; }}
    public bool IsDodgePressed {get { return isDodgePressed; } set { isDodgePressed = value; } }
    public bool IsMovementPressed {get { return isMovementPressed; }}
    public bool IsGlidePressed {get { return isGlidePressed; } set { isGlidePressed = value; } }
    public CharacterController CharacterController{get {return characterController;} set { characterController = value; } }
    public GameObject Raptor {get {return raptor;} set { raptor = value; } }
    public Animator Animator { get { return animator; } }
    public Animator RaptorAnimator { get { return raptorAnimator; } }
    public int IsRunningHash {get { return isRunningHash; }}
    public int IsWalkingHash {get { return isWalkingHash; }}
    public int IsJumpingHash { get { return isJumpingHash; } }
    public int IsFallingHash { get { return isFallingHash; } }
    public int IsGlidingHash { get { return isGlidingHash; } }
    public int IsGroundedHash { get { return isGroundedHash; } }
    public int IsDoubleJumpingHash { get { return isDoubleJumpingHash; } }
    public int IsDodgeHash { get { return isDodgeHash; } }
    public int IsRidingHash { get { return isRidingHash; } }
    public int IsSwimmingHash { get { return isSwimmingHash; } }
    public int IsGeyserGlidingHash { get { return isGeyserGlidingHash; } }
    public float CurrentMovementY { get { return currentMovement.y; } set { currentMovement.y = value; } }
    public float AppliedMovementX { get { return appliedMovement.x; } set { appliedMovement.x = value; } }
    public float AppliedMovementY { get { return appliedMovement.y; } set { appliedMovement.y = value; } }
    public float AppliedMovementZ { get { return appliedMovement.z; } set { appliedMovement.z = value; } }
    public float InitialJumpVelocity { get { return initialJumpVelocity; } set { initialJumpVelocity = value; } }
    public int CurrentNumberOfJumps { get { return currentNumberOfJumps; } set { currentNumberOfJumps = value; } }
    public int MaxNumberOfJumps { get { return maxNumberOfJumps; } set { maxNumberOfJumps = value; } }
    public float Gravity { get { return gravity; } set { gravity = value; } }
    public float GlideGravity { get { return glideGravity; } set { glideGravity = value; } }
    public bool RequireNewJumpPress { get { return requireNewJumpPress; } set { requireNewJumpPress = value; } }
    public bool RequireNewDodgePress { get { return requireNewDodgePress; } set { requireNewDodgePress = value; } }
    public float RunMultiplier { get { return runMultiplier; } set { runMultiplier = value; } }
    public float Speed { get { return speed; } set { speed = value; } }
    public float RaptorSpeed { get { return raptorSpeed; } set { raptorSpeed = value; } }
    public Vector3 Direction { get { return direction; } set { direction = value; } }
    public bool IsDodging { get { return isDodging; } set { isDodging = value; } }
    public float RollSpeed { get { return rollSpeed; } set { rollSpeed = value; } }
    public float RollSpeedTemp { get { return rollSpeedTemp; } set { rollSpeedTemp = value; } }
    public float RollMinimum { get { return rollMinimum; } set { rollMinimum = value; } }
    public bool IsRidePressed { get { return isRidePressed; } set { isRidePressed = value; } }
    public int CurrentNumberOfRaptorJumps { get { return currentNumberOfRaptorJumps; } set { currentNumberOfRaptorJumps = value; } }
    public int MaxNumberOfRaptorJumps {get { return maxNumberOfRaptorJumps; } set { maxNumberOfRaptorJumps = value; } }
    public bool IsInGeyser { get { return isInGeyser; } set { isInGeyser = value; } }
    public float GeyserLiftForce { get { return geyserLiftForce; } set { geyserLiftForce = value; } }
    public bool IsInWater { get { return isInWater; } set { isInWater = value; } }
    public bool IsFloating { get { return isFloating; } set { isFloating = value; } }
    public Camera Camera { get { return cam; } set { cam = value; } }
    public bool RaptorWaterDetection { get { return raptorWaterDetection; } set { raptorWaterDetection = value; } }
}