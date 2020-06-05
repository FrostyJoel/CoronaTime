using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Outline))]
public class InteractableProduct : Interactable {

    public Product scriptableProduct;

    [HideInInspector] public int index;

    public override void Interact(CartStorage cartStorage) {
        if (interactable && currentPlace == Place.InShelve && cartStorage.AddToCart(index)) {
            ProductInteractions.pi_Single.ChangeProductPlace(index, (int)Place.InCart, RpcTarget.All);
            if (SpecialPosAndRot.use) {
                ProductInteractions.pi_Single.SetLocalInteractableProductPositionAndRotation(index, SpecialPosAndRot.pos, Quaternion.Euler(SpecialPosAndRot.rot), RpcTarget.All);
            }
            if (SpecialPosAndRot.productScaling.useNewScale) {
                ProductInteractions.pi_Single.SetLocalInteractableProductScale(index, SpecialPosAndRot.productScaling.scale, RpcTarget.All);
            }
            if (GetComponent<Outline>()) {
                GetComponent<Outline>().enabled = false;
            }
        }
    }
}