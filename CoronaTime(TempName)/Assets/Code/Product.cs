using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class Product : ScriptableObject {
    public GameObject prefab;
    public int scoreValue;
}

public class SoldProduct : ScriptableObject {
    public Product parentProduct;
    public int amountSold;
}
