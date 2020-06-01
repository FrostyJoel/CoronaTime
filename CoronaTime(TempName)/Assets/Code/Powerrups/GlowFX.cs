using Photon.Pun;

public class GlowFX : ThrowPU {

    public override void Effect() {
        if (!inUse) {
            ProductInteractions.pi_Single.EnableDisableControllerOutline(affectedController.photonView.ViewID, true, RpcTarget.All);
        }
    }

    public override void StopUsing() {
        base.StopUsing();
        ProductInteractions.pi_Single.EnableDisableControllerOutline(affectedController.photonView.ViewID, false, RpcTarget.All);
    }
}
