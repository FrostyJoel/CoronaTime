using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UseableProduct : Interactable {

    public PowerUp scriptablePowerUp;
    public int index;
    [Header("HideInInspector")]
    public Controller controller;
    public CartStorage storage;

    private void Update() {
        if (Input.GetButtonDown("UsePowerUp")) {
            Use();
        }   
    }

    public void Use() {
        if(currentPlace == Place.InCart) {
            scriptablePowerUp.Use(controller, storage);
        }
    }

    public override void Interact(CartStorage cartStorage) {
        if (currentPlace == Place.InShelve && cartStorage.SetPowerUp(index)) {
            print("powerup");
            currentPlace = Place.InCart;
            if (GetComponent<Outline>()) {
                GetComponent<Outline>().enabled = false;
            }
        }
    }
}