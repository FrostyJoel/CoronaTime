using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class PhotonProductList : MonoBehaviour {
    public List<InteractableProduct> productList = new List<InteractableProduct>();
    public static List<InteractableProduct> staticProductList = new List<InteractableProduct>();
    [HideInInspector] public bool play;
    private void Awake() {
        play = true;
        staticProductList = productList;
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

        if (GUILayout.Button("Set product list")) {
            InteractableProduct[] interactableProductArray = FindObjectsOfType<InteractableProduct>();
            List<InteractableProduct> ipList = new List<InteractableProduct>();
            photonProductList.productList.Clear();
            for (int i = 0; i < interactableProductArray.Length; i++) {
                InteractableProduct tempProduct = interactableProductArray[i];
                ipList.Add(tempProduct);
                tempProduct.index = i;
                EditorUtility.SetDirty(tempProduct);
            }
            photonProductList.productList = ipList;
        }
    }
}
#endif