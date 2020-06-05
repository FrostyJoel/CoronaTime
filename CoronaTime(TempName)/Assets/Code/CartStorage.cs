using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;

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

    [HideInInspector] public int score;
    [HideInInspector] public Controller controller;
    [HideInInspector] public CartStorage[] storages;
    [HideInInspector] public List<Product> heldProducts = new List<Product>();
    [HideInInspector] public List<GameObject> heldProductModels = new List<GameObject>();
    [HideInInspector] public List<SoldProduct> soldProducts = new List<SoldProduct>();
    [HideInInspector] public List<ScriptScoreboardListing> sbListingsList = new List<ScriptScoreboardListing>();
    public List<Groceries> groceryList = new List<Groceries>();

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
            EnableProductsRelativeToListAndSetUI();
        }
    }

    private void Update() {
        if (photonView.IsMine || controller.playerView.devView) {
            if (Input.GetButtonDown("Interact")) {
                RaycastHit hit;
                if (Physics.Raycast(controller.transform_Pov.position, controller.transform_Pov.forward, out hit, interactRange)) {
                    if (hit.transform.CompareTag("Interact")) {
                        hit.transform.GetComponent<Interactable>().Interact(this);
                    }
                }
            }
        }
    }

    public void UpdateScore() {
        photonView.RPC("RPC_UpdateScoreboardScore", RpcTarget.All, photonView.ViewID, GetScore());
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
            groceryList[indexB].amountGotten++;
            if(groceryList[indexB].amountGotten == groceryList[indexB].amount) {
                groceryList[indexB].groceryListing.text_Grocery.fontStyle = TMPro.FontStyles.Strikethrough;
                List<InteractableProduct> ip_List = ZoneControl.zc_Single.zones[ZoneControl.zc_Single.currentZoneIndex].allProductsInZone;
                for (int i = 0; i < ip_List.Count; i++) {
                    if(ip_List[i].scriptableProduct.productName == groceryList[indexB].product.productName) {
                        ip_List[i].interactable = false;
                    }
                }
            }
            return true;
        } else {
            return false;
        }
    }

    void EnableProductsRelativeToListAndSetUI() {
        if (ZoneControl.zc_Single) {
            Zone zone = ZoneControl.zc_Single.zones[ZoneControl.zc_Single.currentZoneIndex];
            groceryList = zone.groceryList;
            if(transform_GroceryList.childCount > 1) {
                for (int i = transform_GroceryList.childCount; i >= 0 ; i--) {
                    if (transform_GroceryList.GetChild(i).GetComponent<ScriptGroceryListing>()) {
                        Destroy(transform_GroceryList.GetChild(i));
                    }
                }
            }
            for (int i = 0; i < groceryList.Count; i++) {
                GameObject gl = Instantiate(prefab_GroceryListing, transform_GroceryList);
                groceryList[i].groceryListing = gl.GetComponent<ScriptGroceryListing>();
                groceryList[i].groceryListing.text_Grocery.text = zone.groceryListStrings[i];
            }
            for (int i = 0; i < zone.allProductsInZone.Count; i++) {
                int index = InGroceryListWhere(zone.allProductsInZone[i].scriptableProduct);
                if(index >= 0) {
                    zone.allProductsInZone[i].interactable = true;
                }
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

    public int GetCorrespondingSbListing(int id) {
        int index = -1;
        for (int i = 0; i < sbListingsList.Count; i++) {
            if(sbListingsList[i].id == id) {
                index = i;
                break;
            }
        }
        return index;
    }

    int GetScore() {
        int scoreB = 0;
        if(soldProducts.Count > 0) {
            for (int i = 0; i < soldProducts.Count; i++) {
                scoreB += soldProducts[i].parentProduct.scoreValue * soldProducts[i].amount;
            }
        }
        return scoreB;
    }

    Product MakeDirtyNewInstanceOfProduct(Product product) {
        Product tempProduct = ScriptableObject.CreateInstance(product.GetType().Name) as Product;
        tempProduct.prefab = product.prefab;
        tempProduct.scoreValue = product.scoreValue;
        return tempProduct;
    }

    public void ClearProducts() {
        photonView.RPC("RPC_ClearProducts", RpcTarget.All);
    }

    void RPC_NextZone() {
        
    }

    [PunRPC]
    void RPC_UpdateScoreboardScore(int id, int newScore) {
        for (int iB = 0; iB < storages.Length; iB ++) {
            if (storages[iB].photonView.ViewID == id) {
                score = newScore;
                MaxScoreForPresentationCheck.maxScoreFpsSingle.CheckScore(photonView.ViewID);
            } 
            for (int i = 0; i < storages[iB].sbListingsList.Count; i++) {
                if (storages[iB].sbListingsList[i].id == id) {
                    storages[iB].sbListingsList[i].text_Score.text = newScore.ToString();
                }
            }
        }
    }

    [PunRPC]
    void RPC_ClearProducts() {
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
            sbListing.text_Score.text = "0";
            sbListing.id = pv.ViewID;
        }
    }
}