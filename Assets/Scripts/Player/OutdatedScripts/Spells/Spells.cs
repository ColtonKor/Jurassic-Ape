using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spells : MonoBehaviour
{
    public float throwSpeed;
    public float duration = 20f;
    private Rigidbody rb;
    [Header("Light Attack Spells")]
    public float lightDamage;
    public float lightStun;
    [Header("Heavy Attack Spells")]
    public float heavyDamage;
    public float heavyStun;
    public bool isHeavy; 

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
        Destroy(gameObject, duration);
    }
    public void AddImpulse(Vector3 direction)
    {
        rb = GetComponent<Rigidbody>();
        rb.AddForce(direction * throwSpeed, ForceMode.Impulse);
    }
}
