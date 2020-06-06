using Photon.Pun;

public class InteractableCashRegister : Interactable {

    public string hoverText;

    public override void Interact(CartStorage cartStorage) {
        if (cartStorage.heldProducts.Count > 0 && cartStorage.productsGotten == cartStorage.productsNeededInCurrentList) {
            PlaySound();
            cartStorage.score++;
            for (int i = 0; i < cartStorage.heldProductModels.Count; i++) {
                ProductInteractions.pi_Single.DestroyProduct(cartStorage.heldProducts[i].index, 0, RpcTarget.All);
            }
            cartStorage.ClearProducts();
            cartStorage.PhotonUpdateGroceryList(ZoneControl.zc_Single.currentZoneIndex + 1, RpcTarget.All);
            cartStorage.UpdateScore();
        }
    }
}