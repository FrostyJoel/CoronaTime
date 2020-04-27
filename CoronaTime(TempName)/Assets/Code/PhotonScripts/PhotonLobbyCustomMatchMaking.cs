using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhotonLobbyCustomMatchMaking : MonoBehaviourPunCallbacks, ILobbyCallbacks {
    public static PhotonLobbyCustomMatchMaking lobby;

    public string roomName, nickName;
    public int maxPlayers = 4;
    public GameObject roomListingPrefab;
    public Transform roomsPanel;

    private void Awake() {
        lobby = this;
    }

    private void Start() {
        PhotonNetwork.ConnectUsingSettings();
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList) {
        base.OnRoomListUpdate(roomList);
        RemoveRoomListings();
        for (int i = 0; i < roomList.Count; i++) {
            ListRoom(roomList[i]);
        }
    }

    void RemoveRoomListings() {
        while (roomsPanel.childCount > 0) {
            Destroy(roomsPanel.GetChild(0).gameObject);
        }
    }

    void ListRoom(RoomInfo room) {
        if (room.IsOpen && room.IsVisible) {
            GameObject tempListing = Instantiate(roomListingPrefab, roomsPanel);
            RoomButton tempButton = tempListing.GetComponent<RoomButton>();
            tempButton.roomName = room.Name;
            tempButton.roomSize = room.MaxPlayers;
            tempButton.SetRoom();
        }
    }

    public override void OnConnectedToMaster() {
        Debug.Log("Connected to master");
        PhotonNetwork.NickName = nickName + " " + Random.Range(0, 1000);
    }

    public override void OnJoinedRoom() {
        Debug.Log("Joined room");
    }

    public void CreateRoom() {
        RoomOptions options = new RoomOptions() { IsVisible = true, IsOpen = true, MaxPlayers = (byte)maxPlayers };
        PhotonNetwork.CreateRoom(roomName, options);
    }

    public override void OnCreateRoomFailed(short returnCode, string message) {
        Debug.Log("Room already exist");
        //CreateRoom();
    }

    public void OnRoomNameChange(string name) {
        roomName = name;
    }
    
    public void OnNickNameChange(string name) {
        PhotonNetwork.NickName = nickName + " " + Random.Range(0, 1000);
        nickName = name;
    }

    public void OnRoomSizeChange(string size) {
        try {
            maxPlayers = int.Parse(size);
        } catch {
            Debug.Log("string was empty or missing int");
        }
    }
    
    public void JoinLobbyOnCLick() {
        if (!PhotonNetwork.InLobby) {
            PhotonNetwork.JoinLobby();
        }
    }
}
