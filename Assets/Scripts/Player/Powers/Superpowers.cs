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
    [HideInInspector] public float maxCapacity;
    public float damage;
    public float stun;
    [HideInInspector] public float currentCapacity;
    [HideInInspector] public Vector3 direction;
    public Sprite sprite;
}
