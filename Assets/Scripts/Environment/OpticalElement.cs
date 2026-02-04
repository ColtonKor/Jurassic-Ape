using UnityEngine;

public abstract class OpticalElement : MonoBehaviour
{
    public abstract void RegisterLaserBeam(HeatVision laserBeam);

    public abstract void UnregisterLaserBeam(HeatVision laserBeam);

    public abstract void Propagate(HeatVision laserBeam);
}
