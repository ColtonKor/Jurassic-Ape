using UnityEngine;

public class Power : MonoBehaviour
{
    private Rigidbody rb;
    public float lightDamage;
    public float lightStun;
    public float heavyDamage;
    public float heavyStun;
    public float speed;
    [HideInInspector] public bool isHeavy;
    [HideInInspector] public Vector3 direction;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
        Destroy(gameObject, 10);
    }
}
