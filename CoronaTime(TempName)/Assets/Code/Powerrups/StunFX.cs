using Photon.Pun;

public class StunFX : ThrowPU {
    
    public override void Effect() {
        affectedController.currentWalkSpeed *= newValueDuringFX;
        if (!inUse) {
            ProductInteractions.pi_Single.DisableVisibility(index, affectedController.photonView.ViewID, false, RpcTarget.All);
            if (clip) {
                AudioManager.PlaySound(clip, audioGroup);
            }
            inUse = true;
        }
    }
}