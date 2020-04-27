using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using Photon.Pun;

public class RoomButton : MonoBehaviour {
    public Text nameText, sizeText;

    public string roomName;
    public int roomSize;

    public void SetRoom() {
        nameText.text = roomName;
        sizeText.text = roomSize.ToString();
    }

    public void JoinRoomOnClick() {
        PhotonNetwork.JoinRoom(roomName);
    }
}
