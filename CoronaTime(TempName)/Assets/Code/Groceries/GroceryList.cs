using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroceryList : MonoBehaviour {
    ZoneControl zoneControl;
    public string symbolForMultipleProductUse = " x ";

    private void Awake() {
        zoneControl = GetComponent<ZoneControl>();
        Randomize();
    }

    private void Update() {
        if (Input.GetButtonDown("Jump")) {
            Randomize();
        }
    }

    void Randomize() {
        for (int i = 0; i < zoneControl.zones.Length; i++) {
            Zone zone = zoneControl.zones[i];
            zone.groceryList = GetRandomizedGroceryList(zone);
            List<string> stringList = new List<string>();
            for (int iB = 0; iB < zone.groceryList.Count; iB++) {
                string line = zone.groceryList[iB].product.productName;
                if(zone.groceryList[iB].amount > 1) {
                    line += symbolForMultipleProductUse + zone.groceryList[iB].amount;
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
                for (int iB = 0; iB < randomizedList.Count; iB++) {
                    if(randomizedList[iB].product.name == zone.typesOfProductsInZone[random].name) {
                        randomizedList[iB].amount++;
                    } else {
                        randomizedList.Add(new Groceries() { product = zone.typesOfProductsInZone[random], amount = 1 });
                        break;
                    }
                }
            } else {
               randomizedList.Add(new Groceries() { product = zone.typesOfProductsInZone[random], amount = 1 });
            }
        }
        return randomizedList;
    }
}

[System.Serializable]
public class Groceries {
    public Product product;
    public int amount;
}