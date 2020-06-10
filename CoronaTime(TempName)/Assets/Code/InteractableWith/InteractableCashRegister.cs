using Photon.Pun;

public class InteractableCashRegister : Interactable {

    public string hoverText;

    public override void Interact(CartStorage cartStorage) {
        if (cartStorage.heldProducts.Count > 0 && cartStorage.productsGotten == cartStorage.productsNeededInCurrentList) {
            PlaySound();
            cartStorage.score++;
            photonView.RPC("RPC_DestroyHeldProduct", RpcTarget.MasterClient);
            //cartStorage.ClearProducts();
            cartStorage.PhotonUpdateGroceryList(ZoneControl.zc_Single.currentZoneIndex + 1, RpcTarget.All);
            //cartStorage.UpdateScore();
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
                    //for (int iB = 0; iB < storages[i].heldProductModels.Count; iB++) {
                    //    ProductInteractions.pi_Single.DestroyProduct(storages[i].heldProducts[iB].index, 0, RpcTarget.All);
                    //}
                }
            }
        }
    }
}