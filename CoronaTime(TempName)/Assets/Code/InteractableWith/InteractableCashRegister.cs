using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using Photon.Pun;

public class InteractableCashRegister : Interactable {

    public AudioClip sellSound;

    public override void Interact(CartStorage cartStorage) {
        //cartStorage.controller.ResetAtStartPosition();
        if (sellSound) {
            AudioManager.PlaySound(sellSound, audioGroup, transform.position);
        }
        if (cartStorage.heldProducts.Count > 0) {
            for (int i = cartStorage.heldProducts.Count-1; i >= 0; i--) {
                int index = AlreadySold(cartStorage.heldProducts[i], cartStorage);
                if (index >= 0) {
                    cartStorage.soldProducts[index].amount += 1;
                } else {
                    SoldProduct soldProduct_ = new SoldProduct() {
                        parentProduct = cartStorage.heldProducts[i],
                        amount = 1};
                    cartStorage.soldProducts.Add(soldProduct_);
                    //soldProduct_.parentProduct = cartStorage.heldProducts[i];
                    //soldProduct_.amount = 1;
                    //cartStorage.soldProducts.Add(soldProduct_);
                }
                photonView.RPC("RPC_DestroyProduct", RpcTarget.All, cartStorage.heldProducts[i].index);
            }
            cartStorage.ClearProducts();
            cartStorage.UpdateScore();
        }
    }

    int AlreadySold(Product product, CartStorage cartStorage) {
        int index = -1;
        if (cartStorage.soldProducts.Count > 0) {
            for (int i = 0; i < cartStorage.soldProducts.Count; i++) {
                if (cartStorage.soldProducts[i].parentProduct.index == product.index) {
                    index = i;
                    break;
                }
            }
        }
        return index;
    }

    [PunRPC]
    void RPC_DestroyProduct(int index) {
        try
        {
            Destroy(PhotonProductList.staticProductList[index].gameObject);
        }
        catch { }
    }
}