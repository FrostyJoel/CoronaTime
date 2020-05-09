using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhotonPunRpcCallsExample : MonoBehaviour {
    public static PhotonPunRpcCallsExample testSingle;
    public bool bull;

    private void Awake() {
        testSingle = this;
    }

    [PunRPC]
    public void TurnItOn() {
        PhotonRoomCustomMatchMaking.room.PV.RPC("Testt", RpcTarget.All);
    }

    [PunRPC]
    void Testt() {
        bull = !bull;
    }
}
