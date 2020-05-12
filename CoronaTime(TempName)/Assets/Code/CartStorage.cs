using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CartStorage : MonoBehaviourPun {
    public Transform holder;
    [HideInInspector] public Controller controller;
    public int interactRange, maxItemsHeld;

    public List<Transform> itemHolders = new List<Transform>();

    public List<Product> heldProducts = new List<Product>();
    public List<GameObject> heldProductModels = new List<GameObject>();

    public List<SoldProduct> soldProducts = new List<SoldProduct>();

    private void Awake() {
        if (holder) {
            for (int i = 0; i < holder.childCount; i++) {
                itemHolders.Add(holder.GetChild(i));
            }
        }
        controller = GetComponent<Controller>();
    }

    private void Update() {
        //if (controller.playerviewCheck.photonView.isMine || controller.playerviewCheck.devTesting) {
            if (Input.GetButtonDown("Interact")) {
                RaycastHit hit;
                if (Physics.Raycast(controller.pov.position, controller.pov.forward, out hit, interactRange)) {
                    if (hit.transform.CompareTag("Interact")) {
                        hit.transform.GetComponent<Interactable>().Interact(this);
                    }
                }
            }
        //}
    }

    public bool AddToCart(InteractableProduct interactableProduct) {
        if (heldProducts.Count < maxItemsHeld) {
            Product product = MakeDirtyNewInstanceOfProduct(interactableProduct.scriptableProduct);
            heldProducts.Add(product);
            heldProductModels.Add(interactableProduct.gameObject);
            photonView.RPC("RPC_AddToCart", RpcTarget.All, heldProductModels.Count-1);
            return true;
        } else {
            return false;
        }
    }
    GameObject ob = null;
    [PunRPC]
    void RPC_AddToCart(int i) {
        heldProductModels[i].transform.SetParent(itemHolders[heldProducts.Count - 1]);
        heldProductModels[i].transform.localPosition = Vector3.zero;
        heldProductModels[i].transform.localRotation = Quaternion.Euler(Vector3.zero);
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