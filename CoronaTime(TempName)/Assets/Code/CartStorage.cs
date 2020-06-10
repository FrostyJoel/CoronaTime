using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CartStorage : MonoBehaviourPunCallbacks {
    public static CartStorage cartStorageSingle;
    public Transform holder, transform_PowerUpHolder;
    public int interactRange, maxItemsHeld;
    public List<Transform> itemHolders = new List<Transform>();

    [Header("Scoreboard")]
    public Transform transform_Scoreboard;
    public GameObject prefab_ScoreboardListing;

    [Header("Grocerylist")]
    public Transform transform_GroceryList;
    public GameObject prefab_GroceryListing;

    [Header("Interact Mask")]
    public LayerMask mask;

    /*[HideInInspector]*/ public int score;
    [HideInInspector] public Controller controller;
    [HideInInspector] public CartStorage[] storages;
    [HideInInspector] public List<Product> heldProducts = new List<Product>();
    [HideInInspector] public List<GameObject> heldProductModels = new List<GameObject>();
    [HideInInspector] public List<SoldProduct> soldProducts = new List<SoldProduct>();
    [HideInInspector] public List<ScriptScoreboardListing> sbListingsList = new List<ScriptScoreboardListing>();
    [HideInInspector] public List<Groceries> groceryList = new List<Groceries>();
    [HideInInspector] public int productsGotten, productsNeededInCurrentList;

    private void Awake() {
        if (holder) {
            for (int i = 0; i < holder.childCount; i++) {
                itemHolders.Add(holder.GetChild(i));
            }
        }
        controller = GetComponent<Controller>();
    }

    private void Start() {
        if (PhotonNetwork.IsConnected) {
            photonView.RPC("RPC_SetScoreboardListings", RpcTarget.All);
        }
        if (photonView.IsMine || controller.playerView.devView) {
            transform_GroceryList.gameObject.SetActive(true);
        }
            EnableProductsRelativeToListAndSetUI(-1);
    }

    private void Update() {
        if (photonView.IsMine || controller.playerView.devView) {
            if (Input.GetButtonDown("Interact")) {
                RaycastHit hit;
                if(Physics.Raycast(controller.transform_Pov.position, controller.transform_Pov.forward, out hit, interactRange/*, mask*/)){
                    Debug.LogWarning(hit.transform.gameObject.layer);
                }
                if (Physics.Raycast(controller.transform_Pov.position, controller.transform_Pov.forward, out hit, interactRange, mask)) {
                    if (hit.transform.CompareTag("Interact")) {
                        hit.transform.GetComponent<Interactable>().Interact(this);
                    }
                }
            }
        }
    }

    public void UpdateScore() {
        if (photonView.IsMine) {
            photonView.RPC("RPC_UpdateScoreboardScore", RpcTarget.All, photonView.ViewID, productsGotten, productsNeededInCurrentList, score);
        }
    }

    void ClearListing() {
        if (sbListingsList.Count > 0) {
            for (int i = 0; i < sbListingsList.Count; i++) {
                try {
                    Destroy(sbListingsList[i].gameObject);
                } catch { }
            }
        }
        sbListingsList.Clear();
    }

    public bool SetPowerUp(int index) {
        if (!controller.useableProduct) {
            controller.useableProduct = PhotonProductList.staticUseableProductList[index];
            ProductInteractions.pi_Single.SetPowerUp(index, photonView.ViewID, RpcTarget.All);
            return true;
        } else {
            return false;
        }
    }

    public bool AddToCart(int indexA) {
        int indexB = InGroceryListWhere(PhotonProductList.staticInteratableProductList[indexA].scriptableProduct);
        if (heldProducts.Count < maxItemsHeld && indexB >= 0 && groceryList[indexB].amountGotten < groceryList[indexB].amount) {
            ProductInteractions.pi_Single.AddToCart(indexA, photonView.ViewID, RpcTarget.All);
            groceryList[indexB].amountGotten += 1;
            productsGotten += 1;
            if(groceryList[indexB].amountGotten == groceryList[indexB].amount) {
                groceryList[indexB].groceryListing.text_Grocery.fontStyle = FontStyles.Strikethrough;
                List<InteractableProduct> ip_List = ZoneControl.zc_Single.zones[ZoneControl.zc_Single.currentZoneIndex].allProductsInZone;
                for (int i = 0; i < ip_List.Count; i++) {
                    if(ip_List[i].scriptableProduct.productName == groceryList[indexB].product.productName) {
                        ip_List[i].interactable = false;
                    }
                }
            }
            UpdateScore();
            return true;
        } else {
            return false;
        }
    }

    void EnableProductsRelativeToListAndSetUI(int newIndex) {
        if (ZoneControl.zc_Single) {
            int zoneIndex;

            if(newIndex > 0) {
                ZoneControl.zc_Single.currentZoneIndex = newIndex;
            }

            zoneIndex = ZoneControl.zc_Single.currentZoneIndex;

            if (transform_GroceryList.childCount > 1) {
                for (int i = transform_GroceryList.childCount - 1; i > 0; i--) {
                    Destroy(transform_GroceryList.GetChild(i).gameObject);
                }
            }

            if(groceryList.Count > 0) {
                Zone tempZone = ZoneControl.zc_Single.zones[zoneIndex - 1];
                for (int i = 0; i < tempZone.allProductsInZone.Count; i++) {
                    tempZone.allProductsInZone[i].interactable = false;
                }
            }

            if (zoneIndex < ZoneControl.zc_Single.zones.Length) {
                Zone zone = ZoneControl.zc_Single.zones[zoneIndex];
                productsNeededInCurrentList = zone.productsToFind;
                productsGotten = 0;
                groceryList = zone.groceryList;
                if (photonView.IsMine)
                {
                    for (int i = 0; i < groceryList.Count; i++) {
                        GameObject gl = Instantiate(prefab_GroceryListing, transform_GroceryList);
                        groceryList[i].groceryListing = gl.GetComponent<ScriptGroceryListing>();
                        groceryList[i].groceryListing.text_Grocery.text = zone.groceryListStrings[i];
                    }
                }
                for (int i = 0; i < zone.allProductsInZone.Count; i++) {
                    int index = InGroceryListWhere(zone.allProductsInZone[i].scriptableProduct);
                    if(index >= 0) {
                        zone.allProductsInZone[i].interactable = true;
                    }
                }
                UpdateScore();
            } else {
                GameOverCheck.goc_Single.GameOver();
            }
        }
    }

    int InGroceryListWhere(Product productToCheck) {
        int index = -1;
        for (int i = 0; i < groceryList.Count; i++) {
            if(groceryList[i].product.productName == productToCheck.productName) {
                index = i;
                break;
            }
        }
        return index;
    }

    public void ClearProducts() {
        photonView.RPC("RPC_ClearProducts", RpcTarget.All);
    }

    public void PhotonUpdateGroceryList(int zoneIndex, RpcTarget selectedTarget) {
        photonView.RPC("RPC_UpdateGroceryList", selectedTarget, zoneIndex);
    }

    [PunRPC]
    void RPC_UpdateGroceryList(int zoneIndex) {
        for (int i = 0; i < storages.Length; i++) {
            if (storages[i]) {
                storages[i].EnableProductsRelativeToListAndSetUI(zoneIndex);
            }
        }
    }

    [PunRPC]
    void RPC_UpdateScoreboardScore(int id, int gotten, int needed, int newScore) {
        CartStorage stor = PhotonNetwork.GetPhotonView(id).GetComponent<CartStorage>();
        if (stor) {
            stor.score = newScore;
            stor.productsGotten = gotten;
            stor.productsNeededInCurrentList = needed;
        }
        for (int iB = 0; iB < storages.Length; iB++) {
            if (storages[iB]) {
                for (int i = 0; i < storages[iB].sbListingsList.Count; i++) {
                    if (storages[iB].sbListingsList[i].id == id) {
                        storages[iB].sbListingsList[i].text_Score.text = newScore.ToString();
                        storages[iB].sbListingsList[i].text_ItemsInCart.text = gotten + "/" + needed;
                        break;
                    }
                }
            }
        }
    }

    [PunRPC]
    void RPC_ClearProducts() {
        if(heldProductModels.Count > 0 && heldProducts.Count > 0) {
            for (int iB = 0; iB < heldProductModels.Count; iB++) {
                ProductInteractions.pi_Single.DestroyProduct(heldProducts[iB].index, 0, RpcTarget.All);
            }
        }
        heldProducts.Clear();
        heldProductModels.Clear();
    }

    [PunRPC]
    void RPC_SetScoreboardListings() {
        ClearListing();
        storages = FindObjectsOfType<CartStorage>();
        for (int i = 0; i < storages.Length; i++) {
            GameObject sbListingObject = Instantiate(prefab_ScoreboardListing, transform_Scoreboard);
            ScriptScoreboardListing sbListing = sbListingObject.GetComponent<ScriptScoreboardListing>();
            PhotonView pv = storages[i].photonView;
            sbListingsList.Add(sbListing);
            sbListing.text_Username.text = PhotonRoomCustomMatchMaking.roomSingle.RemoveIdFromNickname(pv.Owner.NickName);
            sbListing.text_Score.text = score.ToString();
            sbListing.text_ItemsInCart.text = productsGotten + "/" + productsNeededInCurrentList;
            sbListing.id = pv.ViewID;
        }
    }
}