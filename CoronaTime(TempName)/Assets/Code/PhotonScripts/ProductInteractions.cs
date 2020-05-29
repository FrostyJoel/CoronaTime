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

    public void EnableDisableAllProductColliders(int index, bool state, RpcTarget selectedTarget) {
        photonView.RPC("RPC_EnableDisableAllProductColliders", selectedTarget, index, state);
    }

    public void AddToCart(int index, int id, RpcTarget selectedTarget) {
        photonView.RPC("RPC_AddToCart", selectedTarget, index, id);
    }

    public void SetPowerUp(int index, int id, RpcTarget selectedTarget) {
        photonView.RPC("RPC_SetPowerUp", selectedTarget, index, id);
    }

    public void SetProductPosition(int index, Vector3 pos, Quaternion rot, RpcTarget selectedTarget) { 
        photonView.RPC("RPC_SetProductPosition", selectedTarget, index, pos, rot);
    }

    public void SetParentToPhotonView(int index, int id, RpcTarget selectedTarget) {
        photonView.RPC("RPC_SetParentToPhotonView", selectedTarget, index, id);
    }

    public void DisableLocalVisibility(int index, int id, RpcTarget selectedTarget) {
        photonView.RPC("RPC_DisableLocalVisibility", selectedTarget, index, id);
    }

    public void ChangeProductPlace(int index, int placeIndex, RpcTarget selectedTarget) {
        photonView.RPC("RPC_ChangeProductPlace", selectedTarget, index, placeIndex);        
    }

    public void ChangePowerUpPlace(int index, int placeIndex, RpcTarget selectedTarget) {
        photonView.RPC("RPC_ChangePowerUpPlace", selectedTarget, index, placeIndex);        
    }

    [PunRPC]
    void RPC_DestroyProduct(int index) {
        try {
            Destroy(PhotonProductList.staticInteratableProductList[index].gameObject);
        } catch { }
    }

    [PunRPC]
    void RPC_DestroyUseAbleProduct(int index) {
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

    [PunRPC]
    void RPC_EnableDisableAllProductColliders(int index, bool state) {
        try {
            Collider[] colliders = PhotonProductList.staticUseableProductList[index].gameObject.GetComponentsInChildren<Collider>();
            for (int i = colliders.Length - 1; i >= 0; i--) {
                colliders[i].enabled = state;
            }
        } catch { }
    }

    [PunRPC]
    void RPC_AddToCart(int index, int id) {
        CartStorage storage = PhotonNetwork.GetPhotonView(id).GetComponent<CartStorage>();
        GameObject productObject = PhotonProductList.staticInteratableProductList[index].gameObject;
        storage.heldProducts.Add(PhotonProductList.staticInteratableProductList[index].scriptableProduct);
        storage.heldProductModels.Add(productObject);
        productObject.transform.SetParent(storage.itemHolders[storage.heldProducts.Count - 1]);
        productObject.transform.localPosition = Vector3.zero;
        productObject.transform.localRotation = Quaternion.Euler(Vector3.zero);
    }

    [PunRPC]
    void RPC_SetPowerUp(int index, int id) {
        CartStorage storage = PhotonNetwork.GetPhotonView(id).GetComponent<CartStorage>();
        GameObject productObject = PhotonProductList.staticUseableProductList[index].gameObject;
        productObject.transform.SetParent(storage.transform_PowerUpHolder);
        productObject.transform.localPosition = Vector3.zero;
        productObject.transform.localRotation = Quaternion.Euler(Vector3.zero);
    }

    [PunRPC]
    void RPC_SetProductPosition(int index, Vector3 pos, Quaternion rot) {
        PhotonProductList.staticUseableProductList[index].transform.position = pos;
        PhotonProductList.staticUseableProductList[index].transform.rotation = rot;
    }

    [PunRPC]
    void RPC_SetParentToPhotonView(int index, int id) {
        Transform parentTransform = null;
        if(id >= 0) {
            parentTransform = PhotonNetwork.GetPhotonView(id).transform;
        }
        PhotonProductList.staticUseableProductList[index].transform.SetParent(parentTransform);
    }

    [PunRPC]
    void RPC_DisableLocalVisibility(int index, int id) {
        if (PhotonNetwork.GetPhotonView(id).Owner.IsLocal) {
            PhotonProductList.staticUseableProductList[index].gameObject.layer = (int)Mathf.Sqrt(Manager.staticInformation.dontShowTheseLayersLocal.value) / 4;
        }
    }

    [PunRPC]
    void RPC_ChangeProductPlace(int index, int placeIndex) {
        PhotonProductList.staticInteratableProductList[index].currentPlace = (Interactable.Place)placeIndex;
    }

    [PunRPC]
    void RPC_ChangePowerUpPlace(int index, int placeIndex) {
        PhotonProductList.staticUseableProductList[index].currentPlace = (Interactable.Place)placeIndex;
    }
}