using Photon.Pun;

public class BlockFX : PowerUp {
    public override void Effect() {
        if (!inUse) {
            StartParticle();
            inUse = true;
        }
    }
}