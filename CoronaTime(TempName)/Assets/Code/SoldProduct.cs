using UnityEngine;

public class SoldProduct : ScriptableObject {
    public Product parentProduct;
    public int amount;

    public static SoldProduct MakeInstance(SoldProduct soldProduct) {
        SoldProduct soldProduct1 = ScriptableObject.CreateInstance("SoldProduct") as SoldProduct;

        soldProduct1.name = soldProduct.name;
        soldProduct1.parentProduct = soldProduct.parentProduct;
        soldProduct1.amount = soldProduct.amount;

        return soldProduct1;
    }
}