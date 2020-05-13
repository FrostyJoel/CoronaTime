using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[ExecuteInEditMode]
public class SetProductList : MonoBehaviour {
    PhotonProductList photonProductList;
    private void Start() {
        photonProductList = GetComponent<PhotonProductList>();
        photonProductList.spl = this;
    }

    private void Update() {
        if (photonProductList.setProductList) {
            InteractableProduct[] interactableProductArray = FindObjectsOfType<InteractableProduct>();
            photonProductList.productList.Clear();
            EditorUtility.SetDirty(this);
            for (int i = 0; i < interactableProductArray.Length; i++) {
                photonProductList.productList.Add(interactableProductArray[i]);
                photonProductList.productList[i].index = i;
            }

            photonProductList.setProductList = false;
        }
    }
}