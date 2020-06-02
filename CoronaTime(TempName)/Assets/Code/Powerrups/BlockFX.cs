using Photon.Pun;

public class BlockFX : PowerUp {
    public override void Effect() {
        if (!inUse) {
            ProductInteractions.pi_Single.DisableVisibility(index, affectedController.photonView.ViewID, false, RpcTarget.All);
            inUse = true;
        }
    }
}