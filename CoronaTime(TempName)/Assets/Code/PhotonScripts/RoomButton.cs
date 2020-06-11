using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class RoomButton : MonoBehaviour {
    public Text nameText, sizeText;
    public Button button;
    public string roomName;
    public int roomSize;

    public void SetRoom(RoomInfo room) {
        roomName = room.Name;
        nameText.text = roomName;
        sizeText.text = room.PlayerCount.ToString() + "/" + room.MaxPlayers.ToString();
        button.onClick.AddListener(JoinRoomOnClick);
    }

    public void JoinRoomOnClick() {
        PhotonNetwork.JoinRoom(roomName);
    }
}
