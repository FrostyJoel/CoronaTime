using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZoneControl : MonoBehaviourPun {
    public static ZoneControl zcSingle;
    public List<ZoneType> zoneTypes = new List<ZoneType>();
    [HideInInspector] public List<int> order = new List<int>();

    private void Awake() {
        zcSingle = this;
        for (int i = 0; i < zoneTypes.Count; i++) {
            for (int iB = 0; iB < zoneTypes[i].zones.Length; iB++) {
                if (zoneTypes[i].zones[iB]) {
                    zoneTypes[i].zones[iB].SetActive(false);
                }
            }
        }
    }

    [PunRPC]
    void RandomizeOrder() {
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
        TurnRandomizedZonesOn();
    }

    void TurnRandomizedZonesOn() {
        for (int i = 0; i < zoneTypes.Count; i++) {
            for (int iB = 0; iB < zoneTypes[i].zones.Length; iB++) {
                if (zoneTypes[i].zones[iB]) {
                    zoneTypes[i].zones[iB].SetActive(false);
                }
            }
        }
        for (int i = 0; i < order.Count; i++) {
            if (zoneTypes[i].zones[order[i]]) {
                zoneTypes[i].zones[order[i]].SetActive(true);
            }
        }
    }
}

[System.Serializable]
public class ZoneType {
    public GameObject[] zones = new GameObject[4];
}