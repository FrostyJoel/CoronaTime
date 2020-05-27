using Photon.Pun;
using UnityEngine;

public class ProductInteractions : MonoBehaviourPun {
    public static ProductInteractions pi_Single;

    private void Awake() {
        pi_Single = this;
    }

    public void DestroyProduct(int index, RpcTarget selectedTarget) {
        photonView.RPC("RPC_DestroyProduct", selectedTarget, index);
    }

    public void DestroyUseAbleProduct(int index, RpcTarget selectedTarget) {
        photonView.RPC("RPC_DestroyUseAbleProduct", selectedTarget, index);
    }
    
    public void DestroyAllProductColliders(int index, RpcTarget selectedTarget) {
        photonView.RPC("RPC_DestroyAllProductColliders", selectedTarget, index);
    }

    [PunRPC]
    void RPC_DestroyProduct(int index) {
        try {
            Destroy(PhotonProductList.staticInteratableProductList[index].gameObject);
        } catch { }
    }

    [PunRPC]
    void RPC_DestroyUseAbleProduct (int index) {
        try {
            Destroy(PhotonProductList.staticUseableProductList[index].gameObject);
        } catch { }
    }

    [PunRPC]
    void RPC_DestroyAllProductColliders(int index) {
        try {
            Collider[] colliders = PhotonProductList.staticUseableProductList[index].gameObject.GetComponentsInChildren<Collider>();
            for (int i = colliders.Length - 1; i >= 0; i--) {
                Destroy(colliders[i]);
            }
        } catch { }
    }
}
