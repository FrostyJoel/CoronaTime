using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class PhotonPunRpcCallsExample : MonoBehaviour {
    public static PhotonPunRpcCallsExample testSingle;
    public bool bull;
    public Text text;
    public bool lastBull = true;
    private void Awake() {
        //testSingle = this;
        if(PhotonPunRpcCallsExample.testSingle == null) {
            testSingle = this;
        } else if (PhotonPunRpcCallsExample.testSingle != this) {
            Destroy(PhotonPunRpcCallsExample.testSingle.gameObject);
            PhotonPunRpcCallsExample.testSingle = this;
        }
        DontDestroyOnLoad(this.gameObject);
    }

    private void Update() {
        if (lastBull != PhotonPunRpcCallsExample.testSingle.bull) {
            text.text = PhotonPunRpcCallsExample.testSingle.bull.ToString();
            lastBull = PhotonPunRpcCallsExample.testSingle.bull;
        }
    }

    [PunRPC]
    public void TurnItOn() {
        PhotonRoomCustomMatchMaking.room.PV.RPC("Testt", RpcTarget.All);
    }

    [PunRPC]
    void Testt() {
        PhotonPunRpcCallsExample.testSingle.bull = !PhotonPunRpcCallsExample.testSingle.bull;
    }
}
