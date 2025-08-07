using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerBehaviour : MonoBehaviour
{
    private PlayerControls playerInput;
    private CharacterController characterController;
    private Animator animator;
    private Camera cam;

    private int isWalkingHash;
    private int isRunningHash;
    private int isJumpingHash;
    private int isDoubleJumpingHash;

    private bool isJumpingAnimating;

    private Vector2 currentMovementInput;
    private Vector3 currentMovement;
    private Vector3 appliedMovement;

    private bool isMovementPressed;
    private bool isRunPressed;
    private bool isJumpPressed;
    private bool isAirJumpPressed;


    private float speed = 3f;
    private float runMultiplier = 2f;
    private float rotationFactorPerFrame = 15f;
    private Vector3 direction;

    private float groundedGravity = -0.05f;
    private float gravity = -9.8f;

    private bool isJumping;
    float initialJumpVelocity;
    private float maxJumpHeight = 3f;
    private float maxJumpTime = 1f;

    public int maxNumberOfJumps = 1;
    private int currentNumberOfJumps;

    void Awake()
    {
        playerInput = new PlayerControls();
        characterController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        cam = Camera.main;


        isWalkingHash = Animator.StringToHash("isWalking");
        isRunningHash = Animator.StringToHash("isRunning");
        isJumpingHash = Animator.StringToHash("isJumping");
        isDoubleJumpingHash = Animator.StringToHash("isDoubleJumping");

        playerInput.Player.Move.performed += Move;
        playerInput.Player.Move.started += Move;
        playerInput.Player.Move.canceled += Move;
        playerInput.Player.Sprint.started += Sprint;
        playerInput.Player.Jump.started += Jump;
        playerInput.Player.Jump.canceled += Jump;

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

    public void Jump(InputAction.CallbackContext context)
    {
        isJumpPressed = context.ReadValueAsButton();
        if (!characterController.isGrounded)
        {
            isAirJumpPressed = context.ReadValueAsButton();
        }
    }

    void Update()
    {
        HandleAnimation();
        HandleRotation();
        HandleGravity();
        HandleJump();
        HandleMovement();
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
            direction = Quaternion.Euler(0.0f, cam.transform.eulerAngles.y, 0.0f) * positionToLookAt;
            Vector3 cameraForward = cam.transform.forward;
            cameraForward.y = 0;
            cameraForward.Normalize();
            Quaternion targetRotation = Quaternion.LookRotation(cameraForward, Vector3.up);
            transform.rotation =
                Quaternion.Slerp(currentRotation, targetRotation, rotationFactorPerFrame * Time.deltaTime);
        }
    }

    void HandleMovement()
    {
        float horizontalSpeed = isRunPressed ? speed * runMultiplier : speed;
        appliedMovement.x = direction.x;
        appliedMovement.z = direction.z;

        Vector3 horizontalMovement = new Vector3(appliedMovement.x, 0, appliedMovement.z);
        if (!isMovementPressed)
        {
            horizontalMovement = new Vector3(0, 0, 0);
        }
        Vector3 verticalMovement = new Vector3(0, appliedMovement.y, 0);

        characterController.Move((horizontalMovement * horizontalSpeed + verticalMovement) * Time.deltaTime);
    }

    void HandleJump()
    {
        if (characterController.isGrounded && isJumpPressed)
        {
            animator.SetBool(isJumpingHash, true);
            isJumpingAnimating = true;
            currentMovement.y = initialJumpVelocity;
            appliedMovement.y = initialJumpVelocity;
        } 
        else if (!characterController.isGrounded && isAirJumpPressed && currentNumberOfJumps < maxNumberOfJumps)
        {
            animator.SetBool(isJumpingHash, true);
            animator.SetBool(isDoubleJumpingHash, true);
            isJumpingAnimating = true;
            currentMovement.y = initialJumpVelocity;
            appliedMovement.y = initialJumpVelocity;
            currentNumberOfJumps++;
        }
    }

    void HandleGravity()
    {
        bool isFalling = currentMovement.y <= 0.0f;
        float fallMultiplier = 2f;
        if (characterController.isGrounded)
        {
            if (isJumpingAnimating)
            {
                animator.SetBool(isJumpingHash, false);
                animator.SetBool(isDoubleJumpingHash, false);
                isJumpingAnimating = false;
            }
            
            currentMovement.y = groundedGravity;
            currentNumberOfJumps = 0;
        } 
        else if (isFalling)
        {
            float previousYVelocity = currentMovement.y;
            currentMovement.y += (gravity * fallMultiplier * Time.deltaTime);
            appliedMovement.y = Mathf.Max((previousYVelocity + currentMovement.y) * 0.5f, -20.0f);
        }
        else
        {
            float previousYVelocity = currentMovement.y;
            currentMovement.y += (gravity * Time.deltaTime);
            appliedMovement.y = Mathf.Max((previousYVelocity + currentMovement.y) * 0.5f, -20.0f);
        }
    }

    void HandleAnimation()
    {
        bool isWalking = animator.GetBool(isWalkingHash);
        bool isRunning = animator.GetBool(isRunningHash);

        if (isMovementPressed && !isWalking)
        {
            animator.SetBool(isWalkingHash, true);
        } 
        else if (!isMovementPressed && isWalking)
        {
            animator.SetBool(isWalkingHash, false);
        }

        if ((isMovementPressed && isRunPressed) && !isRunning)
        {
            animator.SetBool(isRunningHash, true);
        }
        else if ((!isMovementPressed || !isRunPressed) && isRunning)
        {
            animator.SetBool(isRunningHash, false);
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
}
