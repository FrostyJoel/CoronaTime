using UnityEngine;

public class SpeedUpFX : PowerUp {

    public override void Effect() {
        affectedController.currentWalkSpeed = newValueDuringFX;
    }

    public override void StopUsing() {
        affectedController.currentWalkSpeed = affectedController.defaultWalkSpeed;
        base.StopUsing();
    }
}
