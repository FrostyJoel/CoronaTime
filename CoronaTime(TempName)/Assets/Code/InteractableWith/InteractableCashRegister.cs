using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using Photon.Pun;

public class InteractableCashRegister : Interactable {

    public override void Interact(CartStorage cartStorage) {
        if (cartStorage.heldProducts.Count > 0 && cartStorage.productsGotten == cartStorage.productsNeededInCurrentList) {
            PlaySound();
            cartStorage.score++;
            cartStorage.PhotonUpdateGroceryList(RpcTarget.All);
            for (int i = 0; i < cartStorage.heldProductModels.Count; i++) {
                ProductInteractions.pi_Single.DestroyProduct(cartStorage.heldProducts[i].index, 0, RpcTarget.All);
            }
            cartStorage.ClearProducts();
            cartStorage.UpdateScore();
        }
    }

    int AlreadySold(Product product, CartStorage cartStorage) {
        int index = -1;
        if (cartStorage.soldProducts.Count > 0) {
            for (int i = 0; i < cartStorage.soldProducts.Count; i++) {
                if (cartStorage.soldProducts[i] && cartStorage.soldProducts[i].parentProduct.index == product.index) {
                    index = i;
                    break;
                }
            }
        }
        return index;
    }
}