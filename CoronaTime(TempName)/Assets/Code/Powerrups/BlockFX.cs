using System.Collections.Generic;
using Photon.Pun;

public class BlockFX : PowerUp {
    public override void Effect() {
        if (!inUse) {
            List<PowerUp> pus = new List<PowerUp>();
            for (int i = 0; i < affectedController.powerups_AffectingMe.Count; i++) {
                pus.Add(affectedController.powerups_AffectingMe[i]);
            }
            pus.Remove(this);
            if (pus.Count > 0) {
                pus[0].StopUsing();
                StopUsing();
            } else {
                StartStopParticle(true);
            }
            ShowFX();
            ProductInteractions.pi_Single.DisableVisibility(index, affectedController.photonView.ViewID, false, RpcTarget.All);
            inUse = true;
        }
    }

    public override void StopUsing() {
        PlayFXSound();
        base.StopUsing();
    }
}