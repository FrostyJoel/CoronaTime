using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class PhotonLobbyCustomMatchMaking : MonoBehaviourPunCallbacks, ILobbyCallbacks {
    public static PhotonLobbyCustomMatchMaking lobby;

    public InputField input_Nickname;
    public Button button_CreateRoom;

    public string roomName, nickName;
    public int maxPlayers = 4;
    public GameObject roomListingPrefab;
    public Transform roomsPanel;

    bool enteredNickname, enteredRoomName, connectedToMaster = false;

    private void Awake() {
        lobby = this;
    }

    private void Start() {
        PhotonNetwork.ConnectUsingSettings();
        PhotonNetwork.AutomaticallySyncScene = true;
        EnableDisableRelativeButtons();
        input_Nickname.text = PlayerPrefs.GetString("NickName");
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList) {
        //base.OnRoomListUpdate(roomList);
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
        //PhotonNetwork.NickName = nickName + " " + Random.Range(0, 1000);
        connectedToMaster = true;
        EnableDisableRelativeButtons();
        PhotonNetwork.JoinLobby();
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
    }

    public void OnNickNameChange(string name) {
        PhotonNetwork.NickName = name + "#" + Random.Range(0, 1000);
        enteredNickname = !string.IsNullOrEmpty(name);
        nickName = name;
        PlayerPrefs.SetString("NickName", name);
        EnableDisableRelativeButtons();

    }

    public void OnRoomNameChange(string name) {
        roomName = name;
        enteredRoomName = !string.IsNullOrEmpty(name);
        EnableDisableRelativeButtons();
    }
    
    void EnableDisableRelativeButtons() {
        button_CreateRoom.interactable = false;
        if (connectedToMaster && enteredNickname) {
            if (enteredRoomName) {
                button_CreateRoom.interactable = true;
            }
        }
    }
}
