using System.Collections.Generic;
using UnityEngine;

public class Zone : MonoBehaviour {
    public int productsToFind = 8;
    public List<string> groceryListStrings = new List<string>();
    [HideInInspector] public List<InteractableProduct> allProductsInZone = new List<InteractableProduct>();
    [HideInInspector] public List<Product> typesOfProductsInZone = new List<Product>();
    [HideInInspector] public List<Groceries> groceryList = new List<Groceries>();
}
