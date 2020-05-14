using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CartStorage : MonoBehaviourPunCallbacks {
    public Transform holder;
    [HideInInspector] public Controller controller;
    public int interactRange, maxItemsHeld;

    public List<Transform> itemHolders = new List<Transform>();

    public List<Product> heldProducts = new List<Product>();
    public List<GameObject> heldProductModels = new List<GameObject>();

    public List<SoldProduct> soldProducts = new List<SoldProduct>();
    public static CartStorage scSingle;
    private void Awake() {
        if (holder) {
            for (int i = 0; i < holder.childCount; i++) {
                itemHolders.Add(holder.GetChild(i));
            }
        }
        controller = GetComponent<Controller>();
        scSingle = this;
    }

    private void Update() {
        //if (controller.playerviewCheck.photonView.isMine || controller.playerviewCheck.devTesting) {
            if (Input.GetButtonDown("Interact")) {
                RaycastHit hit;
                if (Physics.Raycast(controller.pov.position, controller.pov.forward, out hit, interactRange)) {
                    if (hit.transform.CompareTag("Interact")) {
                        Debug.Log("Index " + hit.transform.GetComponent<InteractableProduct>().index);
                        hit.transform.GetComponent<Interactable>().Interact(this);
                    }
                }
            }
        //}
    }

    public bool AddToCart(int index) {
        if (heldProducts.Count < maxItemsHeld) {
            photonView.RPC("RPC_AddToCart", RpcTarget.All, index, photonView.ViewID);
            return true;
        } else {
            return false;
        }
    }

    [PunRPC]
    void RPC_AddToCart(int productListIndex, int id) {
        //Transform productTransform = PhotonProductList.staticProductList[productListIndex].transform;
        if(photonView.ViewID == id) {
            GameObject productObject = PhotonProductList.staticProductList[productListIndex].gameObject;
            heldProducts.Add(PhotonProductList.staticProductList[productListIndex].scriptableProduct);
            heldProductModels.Add(productObject);
            productObject.transform.SetParent(itemHolders[heldProducts.Count - 1]);
            productObject.transform.localPosition = Vector3.zero;
            productObject.transform.localRotation = Quaternion.Euler(Vector3.zero);
        }
        //Debug.Log("Index ");
        //productTransform.localPosition = Vector3.zero;
        //productTransform.localRotation = Quaternion.Euler(Vector3.zero);
        Debug.Log(PhotonView.Find(id).Owner.NickName);
    }

    public void SellItems() {
        if (heldProducts.Count > 0) {
            for (int i = 0; i < heldProducts.Count; i++) {
                int index = AlreadySold(heldProducts[i]);
                if (index >= 0) {
                    soldProducts[index].amount += 1;
                } else {
                    SoldProduct soldProduct_ = new SoldProduct();
                    soldProduct_.parentProduct = heldProducts[i];
                    soldProduct_.amount = 1;
                    soldProducts.Add(soldProduct_);
                }
                Destroy(heldProductModels[i]);
            }
            heldProducts.Clear();
            heldProductModels.Clear();
        }
    }

    int AlreadySold(Product product) {
        int index = -1;
        if (soldProducts.Count > 0) {
            for (int i = 0; i < soldProducts.Count; i++) {
                if(soldProducts[i].parentProduct.prefab == product.prefab) {
                    index = i;
                    break;
                }
            }
        }
        return index;
    }

    public int GetScore() {
        int score = 0;
        if(soldProducts.Count > 0) {
            for (int i = 0; i < soldProducts.Count; i++) {
                score = soldProducts[i].parentProduct.scoreValue * soldProducts[i].amount;
            }
        }
        return score;
    }

    Product MakeDirtyNewInstanceOfProduct(Product product) {
        Product tempProduct = ScriptableObject.CreateInstance(product.GetType().Name) as Product;
        tempProduct.prefab = product.prefab;
        tempProduct.scoreValue = product.scoreValue;
        return tempProduct;
    }
}