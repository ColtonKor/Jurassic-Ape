using UnityEngine;

public class PlayerMovementManager : MonoBehaviour
{
    public bool isSwimming;
    public bool isFloating;
    public bool isWalking;
    public bool isWallClimbing;
    public bool isWallRunning;
    public bool isGliding;
    public bool isGeyserGliding;
    

    private PlayerMovement playerMovement;

    void Start()
    {
        playerMovement = GetComponent<PlayerMovement>();
    }

    public void Update()
    {
        if (!isFloating && !isSwimming)
        {
            isWalking = true;
        }
        else
        {
            isWalking = false;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Water"))
        {
            isFloating = true;
            isWalking = false;
        }    
    }
    
    void OnTriggerStay(Collider other){
        if(other.CompareTag("Geyser") && isGliding){
            isGeyserGliding = true;
        } else if(other.CompareTag("Geyser") && !isGliding){
            isGeyserGliding = false;
        }
    }

    void OnTriggerExit(Collider other){
        if (other.CompareTag("Water"))
        {
            isFloating = false;
            isWalking = true;
        }  
        if(other.CompareTag("Geyser")){
            isGeyserGliding = false;
            if (isGliding){
                StartCoroutine(playerMovement.Fall());
            }
        }
    }
}
