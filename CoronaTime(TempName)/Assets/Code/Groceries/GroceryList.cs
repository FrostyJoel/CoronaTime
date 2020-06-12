using System.Collections.Generic;
using UnityEngine;

public class GroceryList : MonoBehaviour {
    ZoneControl zoneControl;
    public string symbolForMultipleProductUse = " x ";

    private void Awake() {
        zoneControl = GetComponent<ZoneControl>();
        Randomize();
    }

    void Randomize() {
        for (int i = 0; i < zoneControl.zones.Length; i++) {
            Zone zone = zoneControl.zones[i];
            zone.groceryList = GetRandomizedGroceryList(zone);
            List<string> stringList = new List<string>();
            for (int iB = 0; iB < zone.groceryList.Count; iB++) {
                string line = zone.groceryList[iB].product.productName + symbolForMultipleProductUse;
                if(zone.groceryList[iB].amount > 1) {
                    line += zone.groceryList[iB].amount;
                } else {
                    line += 1;
                }
                stringList.Add(line);
            }
            zone.groceryListStrings = stringList;
        }
    }

    List<Groceries>GetRandomizedGroceryList(Zone zone) {
        List<Groceries> randomizedList = new List<Groceries>();
        for (int i = 0; i < zone.productsToFind; i++) {
            int random = Random.Range(0, zone.typesOfProductsInZone.Count);
            if(randomizedList.Count > 0) {
                int index = ContainsInGroceryListWhere(randomizedList, zone.typesOfProductsInZone[random]);
                if(index >= 0) {
                    randomizedList[index].amount++;
                } else {
                    randomizedList.Add(new Groceries() { product = zone.typesOfProductsInZone[random], amount = 1 });
                }
            } else {
               randomizedList.Add(new Groceries() { product = zone.typesOfProductsInZone[random], amount = 1 });
            }
        }
        return randomizedList;
    }

    int ContainsInGroceryListWhere(List<Groceries> gList, Product product) {
        int index = -1;
        for (int iB = 0; iB < gList.Count; iB++) {
            if (gList[iB].product.productName  == product.productName) {
                index = iB;
                break;
            }
        }
        return index;
    }
}

[System.Serializable]
public class Groceries {
    public Product product;
    public int amount;
    public int amountGotten;

    public ScriptGroceryListing groceryListing;
}