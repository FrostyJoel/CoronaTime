using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using Photon.Realtime;
using UnityEngine;

public class Spawnpoints : MonoBehaviourPunCallbacks {
    public static Spawnpoints sp_Single;
    public Transform[] spawnpoints;

    private void Awake() {
        sp_Single = this;
    }

    public Vector3 GetSpPosition(int index) {
        return spawnpoints[index].position;
    }
}