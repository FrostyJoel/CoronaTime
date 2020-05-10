using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class PhotonPunRpcCallsExample : MonoBehaviourPunCallbacks {
    public static PhotonPunRpcCallsExample testSingle;
    public bool bull;
    public Text text;
    public Toggle toggle;

    private void Awake() {
        PhotonPunRpcCallsExample.testSingle = this;
    }

    public void RPC_SetBool(bool state) {
        this.photonView.RPC("Bully", RpcTarget.All, state);
    }

    [PunRPC]
    void Bully(bool b) {
        Debug.Log(b + " bully");
        bull = b;
        text.text = bull.ToString();
        toggle.isOn = b;
    }
}
