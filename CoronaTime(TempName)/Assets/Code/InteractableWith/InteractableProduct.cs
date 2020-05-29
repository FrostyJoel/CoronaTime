using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Outline))]
public class InteractableProduct : Interactable {

    public Product scriptableProduct;
    public int index;

    public override void Interact(CartStorage cartStorage) {
        if (currentPlace == Place.InShelve && cartStorage.AddToCart(index)) {
            ProductInteractions.pi_Single.ChangeProductPlace(index, (int)Place.InCart, RpcTarget.All);
            if (GetComponent<Outline>()) {
                GetComponent<Outline>().enabled = false;
            }
        }
    }
}
