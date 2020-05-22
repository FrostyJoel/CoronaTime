using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class PhotonProductList : MonoBehaviour {
    /*[HideInInspector]*/ public List<InteractableProduct> interactableProductList = new List<InteractableProduct>();
    /*[HideInInspector]*/ public List<UseableProduct> useableProductList = new List<UseableProduct>();
    public static List<InteractableProduct> staticInteratableProductList = new List<InteractableProduct>();
    public static List<UseableProduct> staticUseableProductList = new List<UseableProduct>();
    private void Awake() {
        staticInteratableProductList = interactableProductList;
        staticUseableProductList = useableProductList;
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(PhotonProductList))]
public class PhotonProductListEditor : Editor {
    PhotonProductList photonProductList;
    public void OnEnable() {
        photonProductList = (PhotonProductList)target;
    }
    public override void OnInspectorGUI() {
        DrawDefaultInspector();
        if (GUILayout.Button("Set product list and powerup list")) {
            SetProductList();
            SetPowerUpList();
            Debug.Log("Successfully set lists, don't forget to save!");
        }
    }

    void SetProductList() {
        InteractableProduct[] interactableProductArray = FindObjectsOfType<InteractableProduct>();
        List<InteractableProduct> ipList = new List<InteractableProduct>();
        photonProductList.interactableProductList.Clear();
        for (int i = 0; i < interactableProductArray.Length; i++) {
            InteractableProduct tempProduct = interactableProductArray[i];
            ipList.Add(tempProduct);
            tempProduct.index = i;
            if (tempProduct.scriptableProduct) {
                tempProduct.scriptableProduct = Product.MakeInstance(tempProduct.scriptableProduct);
                tempProduct.scriptableProduct.index = i;
            }
            EditorUtility.SetDirty(tempProduct);
        }
        photonProductList.interactableProductList = ipList;
    }

    void SetPowerUpList() {
        UseableProduct[] useableProductArray = FindObjectsOfType<UseableProduct>();
        List<UseableProduct> upList = new List<UseableProduct>();
        photonProductList.useableProductList.Clear();
        for (int i = 0; i < useableProductArray.Length; i++) {
            UseableProduct tempProduct = useableProductArray[i];
            upList.Add(tempProduct);
            tempProduct.index = i;
            if (tempProduct.scriptablePowerUp) {
                tempProduct.scriptablePowerUp = PowerUp.MakeInstance(tempProduct.scriptablePowerUp);
                tempProduct.scriptablePowerUp.index = i;
            }
            EditorUtility.SetDirty(tempProduct);
        }
        photonProductList.useableProductList = upList;
    }
}
#endif