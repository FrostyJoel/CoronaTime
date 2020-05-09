using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class PhotonPunRpcCallsExample : MonoBehaviourPunCallbacks, IPunObservable {
    public static PhotonPunRpcCallsExample testSingle;
    public bool bull;
    public Text text;

    private void Awake() {
        if (!PhotonPunRpcCallsExample.testSingle) {
            PhotonPunRpcCallsExample.testSingle = this;
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {
        if (stream.IsWriting) {
            stream.SendNext(PhotonPunRpcCallsExample.testSingle.bull);
        } else if (stream.IsReading) {
            SetBull((bool)stream.ReceiveNext());
        }
    }

    public void SetBull(bool state) {
        Debug.Log("bulllll");
        if (PhotonPunRpcCallsExample.testSingle.bull == state) return;

        PhotonPunRpcCallsExample.testSingle.bull = state;
        text.text = state.ToString();
    }





































    //public static PhotonPunRpcCallsExample testSingle;
    //public bool lastBull;
    //private void Awake() {
    //    //testSingle = this;
    //    if(PhotonPunRpcCallsExample.testSingle == null) {
    //        testSingle = this;
    //    } else {
    //        bull = PhotonPunRpcCallsExample.testSingle.bull;
    //        Destroy(PhotonPunRpcCallsExample.testSingle);
    //        PhotonPunRpcCallsExample.testSingle = this;
    //    }
    //    DontDestroyOnLoad(this.gameObject);
    //    lastBull = !bull;
    //}

    //private void Update() {
    //    if (lastBull != PhotonPunRpcCallsExample.testSingle.bull) {
    //        text.text = PhotonPunRpcCallsExample.testSingle.bull.ToString();
    //        lastBull = PhotonPunRpcCallsExample.testSingle.bull;
    //    }
    //}

    //[PunRPC]
    //public void TurnItOn() {
    //    PhotonRoomCustomMatchMaking.room.PV.RPC("Testt", RpcTarget.All);
    //}

    //[PunRPC]
    //void Testt() {
    //    PhotonPunRpcCallsExample.testSingle.bull = !PhotonPunRpcCallsExample.testSingle.bull;
    //}
}
