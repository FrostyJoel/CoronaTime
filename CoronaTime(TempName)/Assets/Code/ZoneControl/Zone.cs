using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zone : MonoBehaviour {
    public int productsToFind = 8;
    [Header("HideInInspector")]
    public List<Product> typesOfProductsInZone = new List<Product>();
    public List<Groceries> groceryList = new List<Groceries>();
    public List<string> groceryListStrings = new List<string>();
}
