using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUp : Product {

    public float durationInSeconds;
    public float durationSpentInSeconds;

    public virtual void SpecialBehaviour() { }

    public virtual void Effect(Controller affectedController, CartStorage affectedCartStorage) {

    }
}

[CreateAssetMenu]
public class PU_SpeedUpUser : PowerUp {

    public override void SpecialBehaviour() {

    }

    public override void Effect(Controller affectedController, CartStorage affectedCartStorage) {
        if(durationSpentInSeconds < durationInSeconds) {
        }
            durationSpentInSeconds += Time.deltaTime;
    }
}