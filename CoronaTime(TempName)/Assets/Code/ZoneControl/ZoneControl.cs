﻿using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(GroceryList))]
public class ZoneControl : MonoBehaviour {
    public static ZoneControl zc_Single;
    public Zone[] zones;

    public int currentZoneIndex = 0;

    private void Awake() {
        currentZoneIndex = 0;
        zc_Single = this;
    }

    public void SetProductsInZone() {
        for (int i = 0; i < zones.Length; i++) {
            InteractableProduct[] temp_IP = zones[i].GetComponentsInChildren<InteractableProduct>();
            List<Product> typesOfProductsInZone = new List<Product>();
            zones[i].allProductsInZone = ArrayToList(temp_IP);
            for (int iB = 0; iB < temp_IP.Length; iB++) {
                Product product = temp_IP[iB].scriptableProduct;
                if (typesOfProductsInZone.Count > 0) {
                    int index = ContainsInListWhere(typesOfProductsInZone, product);
                    if (index == -1) {
                        typesOfProductsInZone.Add(product);
                    }
                } else {
                    typesOfProductsInZone.Add(product);
                }
            }
            zones[i].typesOfProductsInZone = typesOfProductsInZone;
        }
    }

    List<InteractableProduct> ArrayToList(InteractableProduct[] productArray) {
        List<InteractableProduct> productList = new List<InteractableProduct>();
        for (int i = 0; i < productArray.Length; i++) {
            productList.Add(productArray[i]);
        }
        return productList;
    }

    int ContainsInListWhere(List<Product> tempList, Product product) {
        int index = -1;
        for (int i = 0; i < tempList.Count; i++) {
            if(tempList[i].name == product.name) {
                index = i;
                break;
            }
        }
        return index;
    }
}
