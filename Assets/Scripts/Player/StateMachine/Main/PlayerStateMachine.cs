using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

public class PlayerStateMachine : MonoBehaviour
{
    private PlayerControls playerInput;
    private CharacterController characterController;
    private Animator animator;
    private Camera cam;
    public GameObject raptor;
    private Animator raptorAnimator;

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

    void Awake()
    {
        playerInput = new PlayerControls();
        characterController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        raptorAnimator = raptor.GetComponent<Animator>();
        cam = Camera.main;
        
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
        if(characterController.isGrounded || isRidePressed)
        {
            isRidePressed = !isRidePressed;
            HandleMount();
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
        
        if (isMovementPressed)
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