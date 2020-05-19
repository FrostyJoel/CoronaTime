using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZoneControl : MonoBehaviour {
    public static ZoneControl zcSingle;
    public List<ZoneType> zoneTypes = new List<ZoneType>();
    [HideInInspector] public List<int> order = new List<int>();

    private void Awake() {
        zcSingle = this;
    }

    public void RandomizeOrder() {
        order.Clear();
        for (int i = 0; i < zoneTypes.Count; i++) {
            order.Add(i);
        }
        for (int i = 0; i < order.Count; i++) {
            int temp = order[i];
            int randomIndex = Random.Range(i, order.Count);
            order[i] = order[randomIndex];
            order[randomIndex] = temp;
        }
    }
}

[System.Serializable]
public class ZoneType {

    public GameObject zone1;
    public GameObject zone2;
    public GameObject zone3;
    public GameObject zone4;
}