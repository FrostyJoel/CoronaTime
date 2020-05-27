using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StunFX : ThrowPU {
    public override void Hit() {
        base.Hit();
        if (affectedController) {
            affectedController.powerups_AffectingMe.Add(this);
        }
    }

    public override void Effect() {
        
    }
}
