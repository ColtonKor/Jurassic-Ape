using UnityEngine;
using System;


[RequireComponent(typeof(LineRenderer))]
public class HeatVision : Superpowers
{
    public HeatVision prefab;
    
    [SerializeField]private LineRenderer lineRenderer;

    [HideInInspector]public Vector3 StartPosition;
    [HideInInspector]public Vector3 EndPosition;
    [HideInInspector]public Vector3 HitNormal;
    [HideInInspector] public bool deactivateLaser;
    public bool isOriginal;
    
    public Vector3 Direction => (EndPosition - StartPosition).normalized;

    public float damageInterval;
    private float nextDamageTime;

    private Mirror mirrorTheBeamHit;
    
    private OpticalElement opticalElementThatTheBeamHit;
    public OpticalElement OpticalElementThatTheBeamHit { 
        get => opticalElementThatTheBeamHit; 
        set {
            if (opticalElementThatTheBeamHit == value) {
                return;
            }
            else {
                if (opticalElementThatTheBeamHit != null) {
                    opticalElementThatTheBeamHit.UnregisterLaserBeam(this);
                }

                opticalElementThatTheBeamHit = value;

                if (opticalElementThatTheBeamHit != null) {
                    opticalElementThatTheBeamHit.RegisterLaserBeam(this);
                }
            }
        }
    }


    private const float farDistance = 30f;

    
    private void Awake() {                                                                                                               
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
                    else
                    {
                        nextDamageTime = Time.time + damageInterval;
                    }
                }

                if (hit.collider.TryGetComponent(out OpticalElement opticalElement)) {
                    OpticalElementThatTheBeamHit = opticalElement;
                }
                else {
                    OpticalElementThatTheBeamHit = null;
                }
                
                endPosition = hit.point;
                hitNormal = hit.normal;
            }
            else {
                OpticalElementThatTheBeamHit = null;
            }
        }
        
        
        StartPosition = startPosition;
        EndPosition = endPosition;
        HitNormal = hitNormal;
        UpdateVisuals();

        if (OpticalElementThatTheBeamHit) {
            OpticalElementThatTheBeamHit.Propagate(this);
        }
    }

    void UpdateVisuals()
    {
        lineRenderer.SetPosition(0, StartPosition);
        lineRenderer.SetPosition(1, EndPosition);
    }

    public void DeactivatedLaser()
    {
        Debug.Log("Deactivated Laser");
        OpticalElementThatTheBeamHit = null;
    }
    
}
