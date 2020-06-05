using UnityEngine;

[CreateAssetMenu, System.Serializable]
public class Product : ScriptableObject {
    public string productName;
    public GameObject prefab;
    public int scoreValue, index;

    public static Product MakeInstance(Product product) {
        Product product1 = ScriptableObject.CreateInstance("Product") as Product;

        product1.name = product.name;
        product1.productName = product.productName;
        product1.prefab = product.prefab;
        product1.scoreValue = product.scoreValue;

        return product1;
    }
}