using UnityEngine;

public class Superpowers : MonoBehaviour
{
    public enum RangeType
    {
        heatVision,
        sonicScream,
        brainBlast
    } 
    public RangeType rangeType;
    public float maxCapacity;
    public float currentCapacity;
    public float damage;
    public float stun;
}
