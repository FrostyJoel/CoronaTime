using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroceryList : MonoBehaviour {
    public int groceriesPerList;
    public string symbolForMultipleProductUse;
    public List<Product> groceryList = new List<Product>();
    public List<string> groceryStringList = new List<string>();
    List<Groceries> groceries = new List<Groceries>();

    private void Start() {
        RandomizeList();
    }

    private void Update() {
        if (Input.GetButtonDown("Jump")) {
            RandomizeList();
        }
    }

    void RandomizeList() {
        groceryList = GroceryListPool.GetRandomizedGroceryList(groceriesPerList);
        groceries = new List<Groceries>();
        groceryStringList.Clear();
        if (groceryList.Count > 0) {
            for (int i = 0; i < groceryList.Count; i++) {
                int index = WhereInGroceriesList(groceryList[i]);
                if (index >= 0) {
                    groceries[index].amount++;
                } else {
                    groceries.Add(new Groceries() { product = groceryList[i], amount = 1 });
                }
            }

            for (int i = 0; i < groceries.Count; i++) {
                groceryStringList.Add(groceries[i].product.name + symbolForMultipleProductUse + groceries[i].amount);
            }
        }
    }

    int WhereInGroceriesList(Product product) {
        int index = -1;
        if (groceries.Count > 0) {
            for (int iB = 0; iB < groceries.Count; iB++) {
                if (groceries[iB].product.name == product.name) {
                    index = iB;
                }
            }
        }
        return index;
    }
}
[System.Serializable]
public class Groceries {
    public Product product;
    public int amount;
}
