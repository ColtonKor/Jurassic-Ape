using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Mirror : OpticalElement
{
    private List<LaserBeamPair> laserBeamPairs = new List<LaserBeamPair>();

    public override void RegisterLaserBeam(HeatVision laserBeam) {
        HeatVision outgoingLaserBeam = GameObject.Instantiate(laserBeam.prefab, transform);
        laserBeamPairs.Add(new LaserBeamPair(laserBeam, outgoingLaserBeam));
    }
    public override void UnregisterLaserBeam(HeatVision laserBeam) {
        var pair = GetPairFromIncomingBeam(laserBeam);

        if (pair.outgoing.OpticalElementThatTheBeamHit != null) {
            pair.outgoing.OpticalElementThatTheBeamHit.UnregisterLaserBeam(pair.outgoing);
        }

        laserBeamPairs.Remove(pair);
        GameObject.Destroy(pair.outgoing.gameObject);
    }

    public override void Propagate(HeatVision laserBeam) {
        var pair = GetPairFromIncomingBeam(laserBeam);
        Vector3 outgoingDirection = Vector3.Reflect(pair.incoming.Direction, pair.incoming.HitNormal);
        pair.outgoing.Propagate(pair.incoming.EndPosition, outgoingDirection);
    }

    private LaserBeamPair GetPairFromIncomingBeam(HeatVision laserBeam) => laserBeamPairs.Find(x => x.incoming == laserBeam);
}
