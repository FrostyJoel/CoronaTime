using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class PhotonProductList : MonoBehaviour {
    [HideInInspector] public List<InteractableProduct> interactableProductList = new List<InteractableProduct>();
    [HideInInspector] public List<PowerUp> useableProductList = new List<PowerUp>();
    public static List<InteractableProduct> staticInteratableProductList = new List<InteractableProduct>();
    public static List<PowerUp> staticUseableProductList = new List<PowerUp>();
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
            ZoneControl zc = FindObjectOfType<ZoneControl>();
            if (zc) {
                zc.SetProductsInZone();
            }            
            Debug.LogWarning("Successfully set lists, don't forget to save!");
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
            tempProduct.gameObject.layer = 11;
            EditorUtility.SetDirty(tempProduct);
        }
        photonProductList.interactableProductList = ipList;
    }

    void SetPowerUpList() {
        PowerUp[] useableProductArray = FindObjectsOfType<PowerUp>();
        List<PowerUp> upList = new List<PowerUp>();
        photonProductList.useableProductList.Clear();
        for (int i = 0; i < useableProductArray.Length; i++) {
            PowerUp tempProduct = useableProductArray[i];
            upList.Add(tempProduct);
            tempProduct.index = i;
            tempProduct.gameObject.layer = 11;
            EditorUtility.SetDirty(tempProduct);
        }
        photonProductList.useableProductList = upList;
    }
}
#endif