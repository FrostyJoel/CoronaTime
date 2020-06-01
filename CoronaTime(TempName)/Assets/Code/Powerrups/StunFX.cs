using Photon.Pun;

public class StunFX : ThrowPU {
    
    public override void Effect() {
        affectedController.currentWalkSpeed *= newValueDuringFX;
        if (!inUse) {
            ProductInteractions.pi_Single.DisableVisibility(index, affectedController.photonView.ViewID, 0, RpcTarget.All);
            if (clip) {
                AudioManager.PlaySound(clip, audioGroup);
            }
            StartParticle();
            inUse = true;
        }
    }
}