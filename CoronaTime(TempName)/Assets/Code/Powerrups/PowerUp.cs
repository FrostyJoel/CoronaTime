using UnityEngine;

public class PowerUp : ScriptableObject {

    public GameObject prefab;
    public float newValueDuringFX, durationInSeconds, durationSpentInSeconds, index;
    public Controller affectedController;
    public CartStorage affectedCartStorage;
    public bool inUse;

    public static PowerUp MakeInstance(PowerUp pu) {
        PowerUp pu1 = ScriptableObject.CreateInstance("PowerUp") as PowerUp;

        pu1.name = pu.name;
        pu1.prefab = pu.prefab;
        pu1.newValueDuringFX = pu.newValueDuringFX;
        pu1.durationInSeconds = pu.durationInSeconds;
        pu1.index = pu.index;

        return pu1;
    }

    public virtual void Use(Controller controller, CartStorage storage) {
        if (!inUse) {
            affectedController = controller;
            affectedCartStorage = storage;
            affectedController.powerups_AffectingMe.Add(this);
            inUse = true;
        }
    }

    public virtual void Effect() {

    }

    public virtual void StopUsing() {
        affectedController.powerups_AffectingMe.Remove(this);
    }
}

[CreateAssetMenu]
public class SpeedUp : PowerUp {
    
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