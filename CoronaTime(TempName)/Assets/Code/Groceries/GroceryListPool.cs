﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroceryListPool : MonoBehaviour {
    public List<Product> pool = new List<Product>();
    static GroceryListPool groceryListPoolSingle;

    private void Start() {
        groceryListPoolSingle = this;
    }
    //
    public static List<Product> GetRandomizedGroceryList(int itemsNeededOnList) {
        List<Product> tempList = new List<Product>();
        if (groceryListPoolSingle.pool.Count > 0) {
            for (int i = 0; i < itemsNeededOnList; i++) {
                int iB = Random.Range(0, groceryListPoolSingle.pool.Count - 1);
                tempList.Add(Product.MakeProductInstance(groceryListPoolSingle.pool[iB]));
            }
        }
        return tempList;
    }
}