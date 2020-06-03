using System.Collections.Generic;
using UnityEngine;

public class BlockFX : PowerUp {
    public override void Effect() {
        if (!inUse) {
            List<PowerUp> pus = affectedController.powerups_AffectingMe;
            pus.Remove(this);
            if(pus.Count > 0) {
                Debug.LogWarning(">");
                pus[0].StopUsing();
                StopUsing();
            }
            print("Block");
            StartStopParticle(false);
            inUse = true;
        }
    }
}