using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;

public class PowerUp : Interactable {
    public float newValueDuringFX, durationInSeconds;
    [Header("Particle")][Tooltip("Count start 0")]
    public int whatParticleToUse = -1;
    [Space]
    public ParticleDurations standAloneParticle;
    [HideInInspector] public int index;
    [HideInInspector] public Controller affectedController;
    [HideInInspector] public CartStorage affectedCartStorage;
    [HideInInspector] public float durationSpentInSeconds;
    [HideInInspector] public Rigidbody rigid;
    [HideInInspector] public bool inUse, setAsPU;

    public virtual void Use() {
        affectedController.SetAffectingFX(this);
        affectedController.useableProduct = null;
    }

    public virtual void UseEffect() {
        if(durationInSeconds >= 0) {
            if (durationSpentInSeconds < durationInSeconds) {
                Effect();
                durationSpentInSeconds += Time.deltaTime;
            } else {
                StopUsing();
            }
        } else {
            Effect();
        }
    }    

    public virtual void Effect() {

    }

    public void StartParticle() {
        if(whatParticleToUse >= 0) {
            ProductInteractions.pi_Single.StartStopParticleOnPlayer(whatParticleToUse, affectedController.photonView.ViewID, true, RpcTarget.All);
        }
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
        if (whatParticleToUse >= 0) {
            ProductInteractions.pi_Single.StartStopParticleOnPlayer(whatParticleToUse, affectedController.photonView.ViewID, false, RpcTarget.All);
        }
        ProductInteractions.pi_Single.DestroyUseAbleProduct(index, 0, RpcTarget.All);
    }
}