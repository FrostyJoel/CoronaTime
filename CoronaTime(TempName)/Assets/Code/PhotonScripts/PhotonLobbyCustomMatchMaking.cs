﻿using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class PhotonLobbyCustomMatchMaking : MonoBehaviourPunCallbacks, ILobbyCallbacks {
    public static PhotonLobbyCustomMatchMaking lobbySingle;

    public InputField input_Nickname, input_Roomname;
    public Button button_CreateRoom;

    public string roomName, nickName;
    public int maxPlayers = 4;
    public GameObject roomListingPrefab;
    public Transform roomsPanel;

    bool enteredNickname, enteredRoomName, connectedToMaster = false;

    public DevLobby devLobby;

    private void Awake() {
        if (PhotonLobbyCustomMatchMaking.lobbySingle == null) {
            lobbySingle = this;
        } else {
            if (PhotonLobbyCustomMatchMaking.lobbySingle != this) {
                Destroy(PhotonLobbyCustomMatchMaking.lobbySingle.gameObject);
                PhotonLobbyCustomMatchMaking.lobbySingle = this;
            }
        }
        //lobbySingle = this;
    }

    private void Start() {
        if (devLobby.dev) {
            OpenMultiplayer();
            input_Roomname.text = PlayerPrefs.GetString("Roomname");
        }
    }

    public void OpenMultiplayer() {
        PhotonNetwork.ConnectUsingSettings();
        PhotonNetwork.AutomaticallySyncScene = true;
        EnableDisableRelativeButtons();
        input_Nickname.text = PlayerPrefs.GetString("NickName");
    }

    public void LeaveMultiplayer() {
        PhotonNetwork.Disconnect();
    }

    Dictionary<string, RoomButton> m_roomPanelList = new Dictionary<string, RoomButton>();

    public override void OnRoomListUpdate(List<RoomInfo> roomList) {
        foreach (var entry in roomList) {
            if (m_roomPanelList.ContainsKey(entry.Name)) {

                if (entry.RemovedFromList) {
                    RemoveRoomPanel(entry);
                }
            } else {
                if (!entry.RemovedFromList) {
                    AddRoomPanel(entry);
                }
            }
        }
    }

    void AddRoomPanel(RoomInfo room) {
        GameObject gbj = Instantiate(roomListingPrefab, roomsPanel, false);
        RoomButton rb = gbj.GetComponent<RoomButton>();

        rb.SetRoom(room);
        m_roomPanelList.Add(room.Name, rb);
    }

    private void RemoveRoomPanel(RoomInfo room) {
        var panel = m_roomPanelList[room.Name];
        m_roomPanelList.Remove(room.Name);
        Destroy(panel.gameObject);
    }
    //public override void OnRoomListUpdate(List<RoomInfo> roomList) {
    //    Debug.LogWarning("[start]" + GetType() + " " + System.Reflection.MethodInfo.GetCurrentMethod());
    //    //base.OnRoomListUpdate(roomList);
    //    RemoveRoomListings();
    //    for (int i = 0; i < roomList.Count; i++) {
    //        ListRoom(roomList[i]);
    //    }
    //    Debug.LogWarning("[end]" + GetType() + " " + System.Reflection.MethodInfo.GetCurrentMethod());
    //}

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
            //tempButton.SetRoom();
        }
    }

    public override void OnConnectedToMaster() {
        //PhotonNetwork.NickName = nickName + " " + Random.Range(0, 1000);
        connectedToMaster = true;
        EnableDisableRelativeButtons();
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedRoom() {
        Debug.Log("Joined room");
    }

    public void CreateRoom() {
        Debug.LogWarning("[start]" + GetType() + " " + System.Reflection.MethodInfo.GetCurrentMethod());
        RoomOptions options = new RoomOptions() { IsVisible = true, IsOpen = true, MaxPlayers = (byte)maxPlayers };
        PhotonNetwork.CreateRoom(roomName, options);
        Debug.LogWarning("[end]" + GetType() + " " + System.Reflection.MethodInfo.GetCurrentMethod());
    }

    public override void OnCreateRoomFailed(short returnCode, string message) {
        Debug.LogWarning("[start]" + GetType() + " " + System.Reflection.MethodInfo.GetCurrentMethod());
        Debug.Log("Room already exist");
        Debug.LogWarning("[end]" + GetType() + " " + System.Reflection.MethodInfo.GetCurrentMethod());
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
        PlayerPrefs.SetString("Roomname", name);
        EnableDisableRelativeButtons();
    }
    
    void EnableDisableRelativeButtons() {
        Debug.LogWarning("[start]" + GetType() + " " + System.Reflection.MethodInfo.GetCurrentMethod());
        button_CreateRoom.interactable = false;
        if (connectedToMaster && enteredNickname) {
            if (enteredRoomName) {
                button_CreateRoom.interactable = true;
            }
        }
        Debug.LogWarning("[end]" + GetType() + " " + System.Reflection.MethodInfo.GetCurrentMethod());
    }
}

[System.Serializable]
public class DevLobby {
    public bool dev;
}