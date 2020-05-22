using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyProduct : MonoBehaviourPun {
    public static DestroyProduct destroyProduct;

    private void Awake() {
        destroyProduct = this;
    }

    [PunRPC]
    void RPC_DestroyProduct(int index) {
        try {
            Destroy(PhotonProductList.staticInteratableProductList[index].gameObject);
        } catch { }
    }
}
