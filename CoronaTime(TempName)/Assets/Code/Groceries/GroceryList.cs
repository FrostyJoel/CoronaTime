using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroceryList : MonoBehaviour {
    public int groceriesPerList;
    public string signForMultipleProductUse;
    public List<Product> groceryList = new List<Product>();

    private void Update() {
        if (Input.GetButtonDown("Jump")) {
            groceryList = GroceryListPool.GetRandomizedGroceryList(groceriesPerList);
            GroceryListStringsForUiList();
        }
    }

    public string[] GroceryListStringsForUiList() {
        List<string> tempGroceryList = new List<string>();
        List<GroceryLines> tempGroceryLines = new List<GroceryLines>();
    


        for (int i = 0; i < tempGroceryLines.Count; i++) {
            tempGroceryList.Add(tempGroceryLines[i].product.name + "" + signForMultipleProductUse + "" + tempGroceryLines[i].amount.ToString());
            print(tempGroceryLines[tempGroceryLines.Count - 1]);
        }

        return tempGroceryList.ToArray();
    }
}

public class GroceryLines {
    public Product product;
    public int amount;
}
