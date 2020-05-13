using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Outline))]
public class InteractableProduct : Interactable {

    public Product scriptableProduct;

    public enum Place {
        InShelve,
        InCart
    }

    public Place currentPlace;

    private void Awake() {
        scriptableProduct = Product.MakeProductInstance(scriptableProduct);
    }

    public override void Interact(CartStorage cartStorage) {
        if (currentPlace == Place.InShelve && cartStorage.AddToCart(this)) {
            currentPlace = Place.InCart;
            if (GetComponent<Outline>())
            {
                GetComponent<Outline>().enabled = false;
            }
        }
    }
}
