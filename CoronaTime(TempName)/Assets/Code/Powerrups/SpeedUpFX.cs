using Photon.Pun;

public class SpeedUpFX : PowerUp {

    public override void Effect() {
        affectedController.currentWalkSpeed = newValueDuringFX;
        if (!inUse) {
            StartParticle();
            ProductInteractions.pi_Single.DisableVisibility(index, affectedController.photonView.ViewID, false, RpcTarget.All);
            inUse = true;
        }
    }

    public override void StopUsing() {
        affectedController.currentWalkSpeed = affectedController.defaultWalkSpeed;
        base.StopUsing();
    }
}
