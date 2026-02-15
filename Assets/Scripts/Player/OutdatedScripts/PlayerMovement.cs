using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    private Vector2 input;
    private CharacterController cc;
    private AnimationManager ar;
    private OldAttackManager attackManager;
    private PlayerHealth playerHealth;
    private LightningFlury lightning;
    private PlayerMovementManager movementManager;
    private Vector3 direction;
    private float gravity = -9.81f;
    private float velocity;
    private int numberOfJumps;
    private Camera cam;
    [SerializeField] private float jumpPower;
    [SerializeField] private int maxNumberOfJumps = 2;
    [SerializeField] private float gravityMultiplier = 3.0f;
    [SerializeField] private float glideSpeed = 2.0f;
    [SerializeField] private float geyserLiftForce = 10f;
    [SerializeField] private float rotationSpeed = 500f;
    [SerializeField] private float rotationWeapon = 20000f;
    [SerializeField] private float dodgeDistance = 5f;
    [SerializeField] private float dodgeDuration = 0.2f;
    [SerializeField] private Movement movement;
    [Header("Interaction Layers")]
    public LayerMask restLayer;
    public float restRadius = 1f;
    public LayerMask wallLayer;
    public float wallRadius = 1f;
    public LayerMask grappleLayer;
    public float grappleRadius = 10f;
    public LayerMask enemyLayer;
    public float enemyRadius = 10f;
    //End of Interactions
    private bool inGrappleRange = false;
    private bool inRestSpot = false;
    private bool inWall = false;
    private bool lockedOnEnemy = false;
    private bool lightningFlurySprint = false;
    private bool lightningFluryInteract = false;
    private bool isGrappling = false;
    public bool isDodging = false;
    private Vector3 grappleLocation;
    [HideInInspector] public bool grounded;
    
    void Start(){
        attackManager = GetComponent<OldAttackManager>();
        ar = GetComponent<AnimationManager>();
        cc = GetComponent<CharacterController>();
        playerHealth = GetComponent<PlayerHealth>();
        lightning = GetComponent<LightningFlury>();
        movementManager = GetComponent<PlayerMovementManager>();
        cam = Camera.main;
    }

    void Update(){
        grounded = IsGrounded();
        if(IsGrounded() && movementManager.isGliding){
            ar.EndGlide();
            movementManager.isGliding = false;
        }
        if(!isGrappling){
            ApplyRotation();
            if (movementManager.isWalking)
            {
                ApplyGravity();
                ApplyMovement();
            } else 
            {
                ApplySwimmingMovement();
            }
            
        } else {
            ApplyGrappleMovement();
            ApplyGrappleRotation();
        }
        CheckForRest();
        CheckForEnemy();
        CheckForGrapple();
        CheckForWall();
    }

    

    public void Interact(InputAction.CallbackContext context){
        if(context.started){
            if (isDodging)
            {
                return;
            }
            lightningFluryInteract = true;
            if(lightningFlurySprint){
                lightning.Activate();
            } else if(inRestSpot){
                Debug.Log("Resting");
                Rest();
            } else if(inWall){
                Debug.Log("Climb Wall");
            } else if(inGrappleRange){
                Debug.Log("Grappled");
                isGrappling = true;
                movementManager.isGliding = false;
                movementManager.isGeyserGliding = false;
            } else if(lockedOnEnemy){
                Debug.Log("Locked onto the enemy");
            }
        } else if(context.canceled){
            lightningFluryInteract = false;
        }
    }

    private void Rest(){
        playerHealth.RestHeal();
    }

    private void ApplyGravity(){
        if(IsGrounded() && velocity < 0.0f){
            velocity = -1.0f;
        } else {
            if(movementManager.isGliding){
                if (movementManager.isGeyserGliding){
                    velocity = geyserLiftForce;
                } else {
                    velocity = -glideSpeed;
                }
            } else {
                velocity += gravity * gravityMultiplier * Time.deltaTime;
            }
        }
        direction.y = velocity;
    }

    private void ApplyRotation(){
        if(input.sqrMagnitude == 0 && !attackManager.shooting){
            return;
        }
        direction = Quaternion.Euler(0.0f, cam.transform.eulerAngles.y, 0.0f) * new Vector3(input.x, 0.0f, input.y);
        Vector3 cameraForward = cam.transform.forward;
        cameraForward.y = 0;
        cameraForward.Normalize();
        var targetRotation = Quaternion.LookRotation(cameraForward, Vector3.up);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }

    private void ApplyMovement(){
        var targetSpeed = movement.isSprinting ? movement.landMovementSpeed * movement.multiplier : movement.landMovementSpeed;
        movement.currentSpeed = Mathf.MoveTowards(movement.currentSpeed, targetSpeed, movement.acceleration * Time.deltaTime);
        cc.Move(direction * movement.currentSpeed * Time.deltaTime);
    }
    
    private void ApplySwimmingMovement(){
        var targetSpeed = movement.waterMovementSpeed;
        if (movementManager.isFloating)
        {
            movement.currentSpeed = Mathf.MoveTowards(movement.currentSpeed, targetSpeed, movement.acceleration * Time.deltaTime);
            cc.Move(direction * movement.currentSpeed * Time.deltaTime);
        } 
        else if (movementManager.isSwimming)
        {
            
        }
        
    }

    private void ApplyGrappleMovement(){
        transform.position = Vector3.Lerp(transform.position, grappleLocation, Time.deltaTime * movement.landMovementSpeed);
        if (Vector3.Distance(transform.position, grappleLocation) < 0.01f)
        {
            isGrappling = false;
        }
    }

    private void ApplyGrappleRotation(){
        Vector3 direction = (grappleLocation - transform.position).normalized;
        direction.y = 0;
        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }

    public void Move(InputAction.CallbackContext context){
        input = context.ReadValue<Vector2>();
        direction = new Vector3(input.x, 0, input.y);
    }

    public void Jump(InputAction.CallbackContext context){
        if(!context.started){
            return;
        }
        if(!IsGrounded() && numberOfJumps >= maxNumberOfJumps){
            return;
        }
        if (movementManager.isGliding){
            return;
        }
        if (isDodging)
        {
            return;
        }
        if(numberOfJumps == 0){
            StartCoroutine(WaitForLanding());
        }
        numberOfJumps++;
        velocity = jumpPower;
    }

    public void Sprint(InputAction.CallbackContext context){
        if (context.started) {
            if (isDodging)
            {
                return;
            }
            lightningFlurySprint = true;
            if(lightningFluryInteract){
                lightning.Activate();
                return;
            }
            movement.isSprinting = !movement.isSprinting;
        } else if(context.canceled){
            lightningFlurySprint = false;
        }
    }

    public void Glide(InputAction.CallbackContext context){
        if(!IsGrounded() && context.started){
            if (isDodging)
            {
                return;
            }
            if(movementManager.isGliding){
                ar.EndGlide();
                movementManager.isGliding = false;
            } else {
                ar.StartGlide();
                movementManager.isGliding = true;
                velocity = -0.5f;
            }
        }
    }

    public void Dodge(InputAction.CallbackContext context){
        if(context.started || context.performed){
            if (movementManager.isSwimming)
            {
                return;
            }
            if (movementManager.isFloating)
            {
                //Submerge the character
            }
            if (!isDodging)
            {
                StartCoroutine(PerformDodge());
            }
        }
    }

    private IEnumerator PerformDodge(){
        float elapsedTime = 0f;
        Vector3 dodgeDirection = direction.normalized * dodgeDistance;
        dodgeDirection = new Vector3(dodgeDirection.x, 0, dodgeDirection.z);

        while(elapsedTime < dodgeDuration && !(dodgeDirection.x == 0f && dodgeDirection.z == 0) && IsGrounded()){
            isDodging = true;
            cc.Move(dodgeDirection * (Time.deltaTime / dodgeDuration));
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        isDodging = false;
    }

    private IEnumerator WaitForLanding(){
        yield return new WaitUntil(() => !IsGrounded());
        yield return new WaitUntil(IsGrounded);
        numberOfJumps = 0;
    }

    public IEnumerator Fall(){
        yield return new WaitForSeconds(.03f);
        velocity = -0.5f;
    }

    public bool IsGrounded() => cc.isGrounded;

    public void WeaponRotate(){
        direction = Quaternion.Euler(0.0f, cam.transform.eulerAngles.y, 0.0f) * new Vector3(input.x, 0.0f, input.y);
        Vector3 cameraForward = cam.transform.forward;
        cameraForward.y = 0;
        cameraForward.Normalize();
        var targetRotation = Quaternion.LookRotation(cameraForward, Vector3.up);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationWeapon * Time.deltaTime);
    }


    void CheckForRest(){
        Collider[] hits = Physics.OverlapSphere(transform.position, restRadius, restLayer);
        if (hits.Length > 0){
            inRestSpot = true;
        } else {
            inRestSpot = false;
        }
    }

    void CheckForEnemy(){
        Collider[] hits = Physics.OverlapSphere(transform.position, enemyRadius, enemyLayer);
        if (hits.Length > 0){
            lockedOnEnemy = true;
        } else {
            lockedOnEnemy = false;
        }
    }

    void CheckForGrapple(){
        Collider[] hits = Physics.OverlapSphere(transform.position, grappleRadius, grappleLayer);
        if (hits.Length > 0){
            inGrappleRange = true;
            grappleLocation = hits[0].transform.position;
        } else {
            inGrappleRange = false;
        }
    }

    void CheckForWall(){
        Collider[] hits = Physics.OverlapSphere(transform.position, wallRadius, wallLayer);
        if (hits.Length > 0){
            inWall = true;
        } else {
            inWall = false;
        }
    }

    // void OnDrawGizmos(){
    //     Gizmos.color = Color.red;
    //     Gizmos.DrawWireSphere(transform.position, grappleRadius);
    // }
}

[Serializable]
public struct Movement{
     public float landMovementSpeed;
     public float waterMovementSpeed;
     public float glideMovementSpeed;
     public float rotationSpeed;
     public float multiplier;
     public float sprintSpeed;
     public float acceleration;
     public float geyserLiftForce;
     public float jumpPower;
     [HideInInspector]public float velocity;
     [HideInInspector] public bool isSprinting;
     [HideInInspector] public float currentSpeed;
}
