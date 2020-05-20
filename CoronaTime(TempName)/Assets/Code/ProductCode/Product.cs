using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu][System.Serializable]
public class Product : ScriptableObject {
    public GameObject prefab;
    public int scoreValue, index;

    public static Product MakeProductInstance(Product product) {
        Product product1 = ScriptableObject.CreateInstance("Product") as Product;

        product1.name = product.name;
        product1.prefab = product.prefab;
        product1.scoreValue = product.scoreValue;

        return product1;
    }
}

public class SoldProduct : ScriptableObject {
    public Product parentProduct;
    public int amount;

    public static SoldProduct MakeSoldProductInstance(SoldProduct soldProduct) {
        SoldProduct soldProduct1 = ScriptableObject.CreateInstance("SoldProduct") as SoldProduct;

        soldProduct1.name = soldProduct.name;
        soldProduct1.parentProduct = soldProduct.parentProduct;
        soldProduct1.amount = soldProduct.amount;

        return soldProduct1;
    }
}