using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class MainMenu : MonoBehaviourPunCallbacks {
    public GameObject findOpponentPanel = null, waitingStatusPanel = null;
    public Text waitingStatusText = null;

    public string sceneName;

    bool isConnecting = false;
    const string GameVersion = "0.1";
    public int MaxPlayerPerRoom = 2;

    private void Awake() { 
        PhotonNetwork.AutomaticallySyncScene = true; 
    }

    public void FindOpponent() {
        isConnecting = true;
        findOpponentPanel.SetActive(false);
        waitingStatusPanel.SetActive(true);

        waitingStatusText.text = "Searching...";
        if (PhotonNetwork.IsConnected) {
            PhotonNetwork.JoinRandomRoom();
        } else {
            PhotonNetwork.GameVersion = GameVersion;
            PhotonNetwork.ConnectUsingSettings();
        }
    }

    public override void OnConnectedToMaster() {
        Debug.Log("Connected to Master");
        if (isConnecting) {
            PhotonNetwork.JoinRandomRoom();
        }
    }

    public override void OnDisconnected(DisconnectCause cause) {
        waitingStatusPanel.SetActive(false);
        findOpponentPanel.SetActive(true);

        Debug.Log($"Disconnected due to: {cause}");
    }

    public override void OnJoinRandomFailed(short returnCode, string message) {
        Debug.Log("No clients are waiting for an opponent, creating a new room");
        PhotonNetwork.CreateRoom(null, new RoomOptions { MaxPlayers = (byte)MaxPlayerPerRoom });
    }

    public override void OnJoinedRoom() {
        Debug.Log("Client successfully joined a room");
        int playerCount = PhotonNetwork.CurrentRoom.PlayerCount;

        if(playerCount != MaxPlayerPerRoom) {
            waitingStatusText.text = "Waiting for Opponent";
            Debug.Log("Client is waiting for Opponent");
        } else {
            waitingStatusText.text = "Opponent found";
            Debug.Log("Matching is ready to begin");
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer) {
        if(PhotonNetwork.CurrentRoom.PlayerCount == MaxPlayerPerRoom) {
            PhotonNetwork.CurrentRoom.IsOpen = false;

            waitingStatusText.text = "Opponent Found"; 
            Debug.Log("room is full");

            PhotonNetwork.LoadLevel(sceneName);
        }
    }
}
