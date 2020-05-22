using UnityEngine;

public class SpeedUpFX : PowerUp {

    public override void Effect() {
        if (durationSpentInSeconds < durationInSeconds) {
            affectedController.currentWalkSpeed = newValueDuringFX;
            durationSpentInSeconds += Time.deltaTime;
        } else {
            affectedController.currentWalkSpeed = affectedController.defaultWalkSpeed;
            StopUsing();
        }
    }
}
