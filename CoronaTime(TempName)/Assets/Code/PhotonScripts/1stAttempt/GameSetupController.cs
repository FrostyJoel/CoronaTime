using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class GameSetupController : MonoBehaviour {

    public string prefabFolder;
    public GameObject prefab;

    private void Start() {
        CreatePlayer();
    }

    void CreatePlayer() {
        Debug.Log("Creating Player");
        PhotonNetwork.Instantiate(Path.Combine(prefabFolder, prefab.name), Vector3.zero, Quaternion.identity);
        //PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "PhotonPlayer"), Vector3.zero, Quaternion.identity);
    }
}
