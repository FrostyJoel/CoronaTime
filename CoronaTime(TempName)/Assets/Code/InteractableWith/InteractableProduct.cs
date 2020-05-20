using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Outline))]
public class InteractableProduct : Interactable {

    public Product scriptableProduct;
    public int index;

    public enum Place {
        InShelve,
        InCart
    }

    public Place currentPlace;

    public override void Interact(CartStorage cartStorage) {
        if (currentPlace == Place.InShelve && cartStorage.AddToCart(index)) {
            currentPlace = Place.InCart;
            if (GetComponent<Outline>()) {
                GetComponent<Outline>().enabled = false;
            }
        }
    }
}
