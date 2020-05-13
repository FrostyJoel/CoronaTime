using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SetProductList))]
public class PhotonProductList : MonoBehaviour {
    [Tooltip("Only works in edit mode")]public bool setProductList;
    [HideInInspector] public SetProductList spl;
    public List<InteractableProduct> productList = new List<InteractableProduct>();
    public static List<InteractableProduct> staticProductList = new List<InteractableProduct>();

    private void Awake() {
        if (setProductList) {
            setProductList = false;
        }
        spl.enabled = false;
        staticProductList = productList;
    }

    private void Update() {
        if (setProductList) {
            Debug.LogWarning("Can not be used in playmode");
            setProductList = false;
        }
    }
}