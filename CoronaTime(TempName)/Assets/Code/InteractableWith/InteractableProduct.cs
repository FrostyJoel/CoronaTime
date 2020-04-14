using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableProduct : Interactable {

    public Product scriptableProduct;

    public enum Place {
        InShelve,
        InCart
    }

    public Place currentPlace;

    public override void Interact(CartStorage cartStorage) {
        if (currentPlace == Place.InShelve && cartStorage.AddToCart(this)) {
            currentPlace = Place.InCart;
        }
    }
}
