using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZoneControl : MonoBehaviourPun {
    public static ZoneControl zcSingle;
    public List<ZoneType> zoneTypes = new List<ZoneType>();

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
        List<int> newOrder = new List<int>();
        for (int i = 0; i < zoneTypes.Count; i++) {
            newOrder.Add(i);
        }
        for (int i = 0; i < newOrder.Count; i++) {
            int temp = newOrder[i];
            int randomIndex = Random.Range(i, newOrder.Count);
            newOrder[i] = newOrder[randomIndex];
            newOrder[randomIndex] = temp;
        }
        photonView.RPC("TurnOnRandomizedZones", RpcTarget.All, newOrder.ToArray());
    }

    [PunRPC]
    void TurnOnRandomizedZones(int[] newOrder) {
        for (int i = 0; i < zoneTypes.Count; i++) {
            for (int iB = 0; iB < zoneTypes[i].zones.Length; iB++) {
                if (zoneTypes[i].zones[iB]) {
                    zoneTypes[i].zones[iB].SetActive(false);
                }
            }
        }
        for (int i = 0; i < newOrder.Length; i++) {
            if (zoneTypes[i].zones[newOrder[i]]) {
                zoneTypes[i].zones[newOrder[i]].SetActive(true);
            }
        }
    }
}

[System.Serializable]
public class ZoneType {
    public GameObject[] zones = new GameObject[4];
}