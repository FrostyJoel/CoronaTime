using Photon.Pun;
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
        Debug.LogWarning("[start]" + GetType() + " " + System.Reflection.MethodInfo.GetCurrentMethod());
        if (devLobby.dev) {
            OpenMultiplayer();
            input_Roomname.text = PlayerPrefs.GetString("Roomname");
        }
        Debug.LogWarning("[end]" + GetType() + " " + System.Reflection.MethodInfo.GetCurrentMethod());
    }

    public void OpenMultiplayer() {
        Debug.LogWarning("[start]" + GetType() + " " + System.Reflection.MethodInfo.GetCurrentMethod());
        PhotonNetwork.ConnectUsingSettings();
        PhotonNetwork.AutomaticallySyncScene = true;
        EnableDisableRelativeButtons();
        input_Nickname.text = PlayerPrefs.GetString("NickName");
        Debug.LogWarning("[end]" + GetType() + " " + System.Reflection.MethodInfo.GetCurrentMethod());
    }

    public void LeaveMultiplayer() {
        PhotonNetwork.Disconnect();
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList) {
        Debug.LogWarning("[start]" + GetType() + " " + System.Reflection.MethodInfo.GetCurrentMethod());
        //base.OnRoomListUpdate(roomList);
        RemoveRoomListings();
        for (int i = 0; i < roomList.Count; i++) {
            ListRoom(roomList[i]);
        }
        Debug.LogWarning("[end]" + GetType() + " " + System.Reflection.MethodInfo.GetCurrentMethod());
    }

    void RemoveRoomListings() {
        Debug.LogWarning("[start]" + GetType() + " " + System.Reflection.MethodInfo.GetCurrentMethod() + ", Amount of Roomlistings" + roomsPanel.childCount);
        while (roomsPanel.childCount > 0) {
            Destroy(roomsPanel.GetChild(0).gameObject);
        }
        Debug.LogWarning("[end]" + GetType() + " " + System.Reflection.MethodInfo.GetCurrentMethod());
    }

    void ListRoom(RoomInfo room) {
        Debug.LogWarning("[start]" + GetType() + " " + System.Reflection.MethodInfo.GetCurrentMethod());
        if (room.IsOpen && room.IsVisible) {
            GameObject tempListing = Instantiate(roomListingPrefab, roomsPanel);
            RoomButton tempButton = tempListing.GetComponent<RoomButton>();
            tempButton.roomName = room.Name;
            tempButton.roomSize = room.MaxPlayers;
            tempButton.SetRoom();
        }
        Debug.LogWarning("[end]" + GetType() + " " + System.Reflection.MethodInfo.GetCurrentMethod());
    }

    public override void OnConnectedToMaster() {
        Debug.LogWarning("[start]" + GetType() + " " + System.Reflection.MethodInfo.GetCurrentMethod());
        //PhotonNetwork.NickName = nickName + " " + Random.Range(0, 1000);
        connectedToMaster = true;
        EnableDisableRelativeButtons();
        PhotonNetwork.JoinLobby();
        Debug.LogWarning("[end]" + GetType() + " " + System.Reflection.MethodInfo.GetCurrentMethod());
    }

    public override void OnJoinedRoom() {
        Debug.LogWarning("[start]" + GetType() + " " + System.Reflection.MethodInfo.GetCurrentMethod());
        Debug.Log("Joined room");
        Debug.LogWarning("[end]" + GetType() + " " + System.Reflection.MethodInfo.GetCurrentMethod());
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