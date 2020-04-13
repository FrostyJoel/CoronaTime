using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CartStorage : MonoBehaviour {
    public Transform holder;
    Controller controller;
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
        if (Input.GetButtonDown("Interact")) {
            RaycastHit hit;
            if (Physics.Raycast(controller.pov.position, controller.pov.forward, out hit, interactRange)) {
                if (hit.transform.CompareTag("Interact")) {
                    hit.transform.GetComponent<InteractableProduct>().Interact(this);
                }
            }
        }
    }

    public bool AddToCart(InteractableProduct interactableProduct) {
        if (heldProducts.Count < maxItemsHeld) {
            Product product = MakeDirtyNewInstance(interactableProduct.scriptableProduct);
            heldProducts.Add(product);
            heldProductModels.Add(interactableProduct.gameObject);
            interactableProduct.transform.SetParent(itemHolders[heldProducts.Count - 1]);
            interactableProduct.transform.localPosition = Vector3.zero;
            interactableProduct.transform.localRotation = Quaternion.Euler(Vector3.zero);
            return true;
        } else {
            return false;
        }
    }

    Product MakeDirtyNewInstance(Product product) {
        Product tempProduct = ScriptableObject.CreateInstance(product.GetType().Name) as Product;
        tempProduct.prefab = product.prefab;
        tempProduct.scoreValue = product.scoreValue;
        return tempProduct;
    }
}