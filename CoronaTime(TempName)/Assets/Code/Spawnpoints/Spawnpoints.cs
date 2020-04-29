using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawnpoints : MonoBehaviour {
    public bool generateSpawnpoints;
    public GameObject spawnpointPrefab;
    public List<Spawnpoint> spawnpoints = new List<Spawnpoint>();

    [PunRPC]
    private void Start() {
        if (generateSpawnpoints) {
            for(int i = 0; i < PhotonLobbyCustomMatchMaking.lobby.maxPlayers; i++) {
                GameObject g = PhotonNetwork.Instantiate(spawnpointPrefab.name, Vector3.zero, Quaternion.identity);
                spawnpoints.Add(g.GetComponentInChildren<Spawnpoint>());
                g.transform.SetParent(transform);
            }
        }
    }
}