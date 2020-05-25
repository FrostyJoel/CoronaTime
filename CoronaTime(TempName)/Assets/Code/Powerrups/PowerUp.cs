using Photon.Pun;
using UnityEngine;

public class PowerUp : Interactable {
    public float newValueDuringFX, durationInSeconds;
    [Header("HideInInspector")]
    public int index;
    public Controller affectedController;
    public CartStorage affectedCartStorage;
    public float durationSpentInSeconds;

    public virtual void Use() {
        affectedController.powerups_AffectingMe.Add(this);
        affectedController.useableProduct = null;
    }

    public virtual void Effect() {

    }

    public override void Interact(CartStorage cartStorage) {
        if (currentPlace == Place.InShelve && cartStorage.SetPowerUp(index)) {
            print("Interact");
            currentPlace = Place.InCart;
            affectedCartStorage = cartStorage;
            affectedController = cartStorage.controller;
            if (GetComponent<Outline>()) {
                GetComponent<Outline>().enabled = false;
            }
        }
    }

    public virtual void StopUsing() {
        affectedController.powerups_AffectingMe.Remove(this);
        DestroyProduct.destroyProduct.photonView.RPC("RPC_DestroyUseAbleProduct", RpcTarget.All, index);
    }
}