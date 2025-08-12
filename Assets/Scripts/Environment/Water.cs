using UnityEngine;

public class Water : MonoBehaviour
{
    private PlayerStateMachine Ctx;
    public bool floatDevice;
    public bool raptorDetection;
    void Start()
    {
        Ctx = GetComponentInParent<PlayerStateMachine>();
    }
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Water"))
        {
            if (raptorDetection)
            {
                Ctx.RaptorWaterDetection = true;
                return;
            }
            // if (!floatDevice)
            // {
            //     Ctx.IsInWater = true;
            //     Ctx.IsFloating = false;
            // }
            // else
            // {
                // Ctx.IsFloating = true;
            // }
            Ctx.IsInWater = true;
            Ctx.IsFloating = true;
        }
    }
    
    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Water"))
        {
            if (floatDevice && Ctx.IsInWater)
            {
                Ctx.IsFloating = true;
            }
            else
            {
                Ctx.IsFloating = false;
                Ctx.IsInWater = false;
            }
        }
    }
}
