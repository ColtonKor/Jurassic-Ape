using UnityEngine;

public class LaserBeamPair {
    public HeatVision incoming;
    public HeatVision outgoing;

    public LaserBeamPair(HeatVision incoming, HeatVision outgoing) {
        this.incoming = incoming;
        this.outgoing = outgoing;
    }
}

