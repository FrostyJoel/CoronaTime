using Photon.Pun;

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

    [PunRPC]
    void RPC_DestroyUseAbleProduct (int index) {
        try {
            Destroy(PhotonProductList.staticUseableProductList[index].gameObject);
        } catch { }
    }
}
