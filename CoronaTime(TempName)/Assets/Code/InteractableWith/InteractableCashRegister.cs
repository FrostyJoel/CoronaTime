using Photon.Pun;

public class InteractableCashRegister : Interactable {

    public string hoverText;

    public override void Interact(CartStorage cartStorage) {
        if (cartStorage.heldProducts.Count > 0 && cartStorage.productsGotten == cartStorage.productsNeededInCurrentList) {
            PlayInteractSound();
            cartStorage.score++;
            cartStorage.PhotonUpdateGroceryList(ZoneControl.zc_Single.currentZoneIndex + 1, RpcTarget.All);
            photonView.RPC("RPC_DestroyHeldProduct", RpcTarget.MasterClient);
        }
    }

    [PunRPC]
    void RPC_DestroyHeldProduct() {
        if (photonView.IsMine) {
            CartStorage[] storages = FindObjectsOfType<CartStorage>();
            for (int i = 0; i < storages.Length; i++) {
                if (storages[i]) {
                    storages[i].ClearProducts();
                    storages[i].UpdateScore();
                }
            }
        }
    }
}