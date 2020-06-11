using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Test : MonoBehaviour {
    public GameObject gamemanagerPrefab;
    public Options options;

    private void Start() {
        PhotonView pv = Instantiate(gamemanagerPrefab).GetComponent<PhotonView>();
        pv.ViewID = 999;
    }
}
