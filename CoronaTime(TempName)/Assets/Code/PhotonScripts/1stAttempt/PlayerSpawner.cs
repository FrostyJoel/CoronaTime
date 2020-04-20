using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpawner : MonoBehaviour {
    public GameObject prefab;
    private void Start() {
        PhotonNetwork.Instantiate(prefab.name, Vector3.zero, Quaternion.identity);
    }
}
