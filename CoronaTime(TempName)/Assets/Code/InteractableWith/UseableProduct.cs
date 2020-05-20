using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UseableProduct : InteractableProduct {

    public virtual void Use() {  }

    public override void Interact(CartStorage cartStorage) {
        if (currentPlace == Place.InShelve && cartStorage.SetPowerUp(index)) {
            currentPlace = Place.InCart;
            if (GetComponent<Outline>()) {
                GetComponent<Outline>().enabled = false;
            }
        }
    }
}