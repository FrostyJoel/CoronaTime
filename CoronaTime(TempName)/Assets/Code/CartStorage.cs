using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;

public class CartStorage : MonoBehaviourPunCallbacks {
    public static CartStorage cartStorageSingle;
    public Transform holder, transform_PowerUpHolder;
    [HideInInspector] public Controller controller;
    public int interactRange, maxItemsHeld, score;

    public List<Transform> itemHolders = new List<Transform>();
    [HideInInspector] public List<Product> heldProducts = new List<Product>();
    [HideInInspector] public List<GameObject> heldProductModels = new List<GameObject>();
    [HideInInspector] public List<SoldProduct> soldProducts = new List<SoldProduct>();
    
    [Header("Scoreboard")]
    public Transform transform_Scoreboard;
    public GameObject prefab_ScoreboardListing;

    [Header("HideInInspector")]
    public List<ScriptScoreboardListing> sbListingsList = new List<ScriptScoreboardListing>();
    public CartStorage[] storages;
    public PowerUp currentPowerUp;

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
            photonView.RPC("RPC_SetPowerUp", RpcTarget.All, index, photonView.ViewID);
            return true;
        } else {
            return false;
        }
    }

    public bool AddToCart(int index) {
        if (heldProducts.Count < maxItemsHeld) {
            photonView.RPC("RPC_AddToCart", RpcTarget.All, index, photonView.ViewID);
            return true;
        } else {
            return false;
        }
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
    void RPC_SetPowerUp(int index, int id) {
        if (photonView.ViewID == id) {
            GameObject productObject = PhotonProductList.staticUseableProductList[index].gameObject;
            productObject.transform.SetParent(transform_PowerUpHolder);
            productObject.transform.localPosition = Vector3.zero;
            productObject.transform.localRotation = Quaternion.Euler(Vector3.zero);
        }
    }

    [PunRPC]
    void RPC_AddToCart(int index, int id) {
        if(photonView.ViewID == id) {
            GameObject productObject = PhotonProductList.staticInteratableProductList[index].gameObject;
            heldProducts.Add(PhotonProductList.staticInteratableProductList[index].scriptableProduct);
            heldProductModels.Add(productObject);
            productObject.transform.SetParent(itemHolders[heldProducts.Count - 1]);
            productObject.transform.localPosition = Vector3.zero;
            productObject.transform.localRotation = Quaternion.Euler(Vector3.zero);
        }
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