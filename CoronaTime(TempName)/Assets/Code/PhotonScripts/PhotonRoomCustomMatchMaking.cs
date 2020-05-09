﻿using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PhotonRoomCustomMatchMaking : MonoBehaviourPunCallbacks, IInRoomCallbacks {
    public static PhotonRoomCustomMatchMaking room;
    public GameObject playerPrefab, lobbyGameObject, roomGameObject, playerListingPrefab, startButton;
    public Transform playersPanel;
    public PhotonView PV;

    public bool isLoaded;
    public int currentScene;
    [Space]
    Player[] photonPlayers;
    public int playersInRoom, myNumberInRoom;
    [Space]
    public int playersInGame;

    private void Awake() {
        if (PhotonRoomCustomMatchMaking.room == null) {
            room = this;
        } else {
            if(PhotonRoomCustomMatchMaking.room != this) {
                Destroy(PhotonRoomCustomMatchMaking.room.gameObject);
                PhotonRoomCustomMatchMaking.room = this;
            }
        }
        DontDestroyOnLoad(this.gameObject);
    }

    public override void OnEnable() {
        PhotonNetwork.AddCallbackTarget(this);
        SceneManager.sceneLoaded += OnSceneFinishedLoading;
    }

    public override void OnDisable() {
        PhotonNetwork.RemoveCallbackTarget(this);
        SceneManager.sceneLoaded -= OnSceneFinishedLoading;
    }

    private void Start() {
        PV = GetComponent<PhotonView>();
    }

    public override void OnJoinedRoom() {
        base.OnJoinedRoom();
        Debug.Log("Joined room");
        photonPlayers = PhotonNetwork.PlayerList;
        playersInRoom = photonPlayers.Length;
        CLearPlayerListings();
        ListPlayers();
        if (lobbyGameObject) {
            lobbyGameObject.SetActive(false);
        }
        if (roomGameObject) {
            roomGameObject.SetActive(true);
        }
        myNumberInRoom = playersInRoom;
    }

    public override void OnPlayerEnteredRoom(Player newPlayer) {
        base.OnPlayerEnteredRoom(newPlayer);
        Debug.Log("player joined the room");
        CLearPlayerListings();
        ListPlayers();
        if (lobbyGameObject) {
            lobbyGameObject.SetActive(false);
        }
        if (roomGameObject) {
            roomGameObject.SetActive(true);
        }
        if (startButton) {
            if (PhotonNetwork.IsMasterClient) {
                startButton.SetActive(true);
            } else {
                startButton.SetActive(false);
            }
        }

        CLearPlayerListings();
        ListPlayers();

        playersInRoom++;
    }

    void CLearPlayerListings() {
        if (playersPanel) {
            for (int i = playersPanel.childCount - 1; i >= 0; i--) {
                Destroy(playersPanel.GetChild(i).gameObject);
            }
        }
    }

    void ListPlayers() {
        if (PhotonNetwork.InRoom) {
            for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++) {
                GameObject tempNickNameObject = Instantiate(playerListingPrefab, playersPanel);
                Text tempNickNameText = tempNickNameObject.transform.GetChild(0).GetComponent<Text>();
                List<char> characterList = CharArrayToList(PhotonNetwork.PlayerList[i].NickName.ToCharArray());
                string nickName = "";
                for (int iB = 0; iB < characterList.Count; iB++) {
                    if (characterList[iB].ToString() == "#") {
                        break;
                    }
                    nickName = nickName + characterList[iB];
                }
                tempNickNameText.text = nickName;
            }
        }
    }

    List<char> CharArrayToList(char[] chars) {
        List<char> tempList = new List<char>();
        for (int i = 0; i < chars.Length; i++) {
            tempList.Add(chars[i]);
        }
        return tempList;
    }

    public void StartGame() {
        isLoaded = true;
        PhotonNetwork.LoadLevel(currentScene + 1);
        PhotonNetwork.CurrentRoom.IsOpen = false;
    }

    void OnSceneFinishedLoading(Scene scene, LoadSceneMode mode) {
        currentScene = scene.buildIndex;
        if(currentScene == MultiplayerSetting.multiplayerSetting.multiplayerScene) {
            isLoaded = true;
            PV.RPC("RPC_LoadedGameScene", RpcTarget.MasterClient);
        }
    }

    public override void OnPlayerLeftRoom(Player otherPlayer) {
        base.OnPlayerLeftRoom(otherPlayer);
        Debug.Log(otherPlayer.NickName + " left the game");
        playersInRoom--;
        CLearPlayerListings();
        ListPlayers();
    }

    [PunRPC]
    void RPC_LoadedGameScene() {
        playersInGame++;
        if(playersInGame == PhotonNetwork.PlayerList.Length) {
            PV.RPC("RPC_CreatePlayer", RpcTarget.All);
        }
    }

    [PunRPC]
    void RPC_CreatePlayer() {
        Vector3 pos = Vector3.zero;
        pos = new Vector3(0, 1, 4);
        //pos = Spawnpoints.spawnpointsSingleton.spawnpoints[0].actualSpawnpoint.position;
        GameObject g = PhotonNetwork.Instantiate(PhotonRoomCustomMatchMaking.room.playerPrefab.name, pos, Quaternion.identity);
        DontDestroyOnLoad(g);
    }
}