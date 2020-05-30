using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StunFX : ThrowPU {
    
    public override void Effect() {
        affectedController.currentWalkSpeed *= newValueDuringFX;
        if (!inUse) {
            AudioManager.PlaySound(clip, audioGroup);
            StartParticle();
            inUse = true;
        }
    }
}
