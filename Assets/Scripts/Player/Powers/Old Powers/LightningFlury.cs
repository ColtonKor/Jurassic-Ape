using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightningFlury : MonoBehaviour
{
    public float maxBuildUp = 100f;
    public float currentBuildUp;
    
    public void Activate(){
        if(currentBuildUp == maxBuildUp){
            Debug.Log("Activate Lightning Flury");
        }
    }
}
