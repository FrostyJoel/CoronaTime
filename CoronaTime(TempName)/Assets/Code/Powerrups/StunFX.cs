using Photon.Pun;

public class StunFX : ThrowPU {
    
    public override void Effect() {
        affectedController.currentWalkSpeed *= newValueDuringFX;
        if (!inUse) {
            StartStopParticle(true);
            ProductInteractions.pi_Single.DisableVisibility(index, affectedController.photonView.ViewID, false, RpcTarget.All);
            PlayFXSound();
            ShowFX();
            inUse = true;
        }
    }
}