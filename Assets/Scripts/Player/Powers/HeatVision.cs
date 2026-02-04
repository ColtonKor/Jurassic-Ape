using UnityEngine;

public class HeatVision : Superpowers
{
    [SerializeField]private LineRenderer lineRenderer;

    [HideInInspector]public Vector3 StartPosition;
    [HideInInspector]public Vector3 EndPosition;
    [HideInInspector]public Vector3 HitNormal;

    public float damageInterval;
    private float nextDamageTime;


    private const float farDistance = 30f;
    
    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = 2;
    }

    public void Propagate(Vector3 startPosition, Vector3 direction)
    {
        Vector3 endPosition = startPosition + direction * farDistance;
        Vector3 hitNormal = Vector3.zero;

        if (Physics.Raycast(startPosition, direction, out RaycastHit hit, farDistance))
        {
            if(!hit.collider.CompareTag("Player"))
            {
                if (Time.time >= nextDamageTime)
                {
                    EnemyHealth enemy = hit.collider.GetComponent<EnemyHealth>();
                    if (enemy != null)
                    {
                        enemy.TakeDamage(damage);
                        enemy.TakeStun(stun);

                        nextDamageTime = Time.time + damageInterval;
                    }
                }
                
                endPosition = hit.point;
                hitNormal = hit.normal;
            }
            else
            {
                nextDamageTime = Time.time + damageInterval;
            }
        }
        
        StartPosition = startPosition;
        EndPosition = endPosition;
        HitNormal = hitNormal;
        UpdateVisuals();
    }

    void UpdateVisuals()
    {
        lineRenderer.SetPosition(0, StartPosition);
        lineRenderer.SetPosition(1, EndPosition);
    }
    
}
