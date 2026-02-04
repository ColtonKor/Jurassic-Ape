using UnityEngine;

public class LaserSource : MonoBehaviour
{
    public Transform sourcePosition;
    public HeatVision heatVision;

    private Camera cam;
    
    void Start()
    {
        cam = Camera.main;
    }

    private void Update()
    {
        Vector3 startPosition = sourcePosition.position;
        Vector3 direction = cam.transform.forward;
        
        heatVision.Propagate(startPosition, direction);
    }
}
