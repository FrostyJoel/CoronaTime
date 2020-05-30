using Photon.Pun;
using UnityEngine;

public class PowerUp : Interactable {
    public float newValueDuringFX, durationInSeconds;
    [Header("Particle")][Tooltip("Count start 0")]
    public int whatParticleToUse;
    [HideInInspector] public int index;
    [HideInInspector] public Controller affectedController;
    [HideInInspector] public CartStorage affectedCartStorage;
    [HideInInspector] public float durationSpentInSeconds;
    [HideInInspector] public Rigidbody rigid;
    [Space]
    public bool inUse;

    public virtual void Use() {
        affectedController.powerups_AffectingMe.Add(this);
        affectedController.useableProduct = null;
    }

    public virtual void UseEffect() {
        if (durationSpentInSeconds < durationInSeconds) {
            Effect();
            print("Effect " + durationSpentInSeconds + " / " + durationInSeconds);
            durationSpentInSeconds += Time.deltaTime;
        } else {
            StopUsing();
            print("Stop using");
        }
    }

    public virtual void Effect() {

    }

    public void StartParticle() {
        affectedController.particles[whatParticleToUse].dur = durationInSeconds;
        affectedController.particles[whatParticleToUse].play = true;
    }

    public override void Interact(CartStorage cartStorage) {
        if (currentPlace == Place.InShelve && cartStorage.SetPowerUp(index)) {
            ProductInteractions.pi_Single.ChangePowerUpPlace(index, (int)Place.InCart, RpcTarget.All);
            affectedCartStorage = cartStorage;
            affectedController = cartStorage.controller;
            if (GetComponent<Outline>()) {
                GetComponent<Outline>().enabled = false;
            }
        }
    }

    public virtual void StopUsing() {
        affectedController.powerups_AffectingMe.Remove(this);
        ProductInteractions.pi_Single.DestroyUseAbleProduct(index, RpcTarget.All);
    }
}