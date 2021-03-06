﻿using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PhotonRoomCustomMatchMaking : MonoBehaviourPunCallbacks, IInRoomCallbacks {
    public static PhotonRoomCustomMatchMaking roomSingle;
    public GameObject playerPrefab, lobbyGameObject, roomGameObject, playerListingPrefab, startButton, loadingTextObject;
    public Transform playersPanel;
    [HideInInspector] public PhotonView PV;

    public bool isLoaded;
    public int currentScene;
    [Space]
    Player[] photonPlayers;
     
    [Header("HideInInspector")]
    public int playersInGame, playersInRoom, myNumberInRoom;

    private void Awake() {
        if (PhotonRoomCustomMatchMaking.roomSingle == null) {
            PhotonRoomCustomMatchMaking.roomSingle = this;
        } else {
            if(PhotonRoomCustomMatchMaking.roomSingle != this) {
                Destroy(PhotonRoomCustomMatchMaking.roomSingle.gameObject);
                PhotonRoomCustomMatchMaking.roomSingle = this;
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
        photonPlayers = PhotonNetwork.PlayerList;
        playersInRoom = photonPlayers.Length;
        ClearPlayerListings();
        myNumberInRoom = playersInRoom;
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
    }

    public override void OnPlayerEnteredRoom(Player newPlayer) {
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

        ClearPlayerListings();
        ListPlayers();

        playersInRoom++;
    }

    void ClearPlayerListings() {
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
                ScriptPlayerListing spl = tempNickNameObject.GetComponent<ScriptPlayerListing>();
                string nickname = RemoveIdFromNickname(PhotonNetwork.PlayerList[i].NickName);
                spl.text_Nickname.text = nickname;
            }
        }
    }

    public string RemoveIdFromNickname(string nickname) {
        return RemoveIdFromNickname(CharArrayToList(nickname.ToCharArray()));
    }

    public string RemoveIdFromNickname(List<char> characterList) {
        string nickName = "";
        for (int iB = 0; iB < characterList.Count; iB++) {
            if (characterList[iB].ToString() == "#") {
                break;
            }
            nickName += characterList[iB];
        }
        return nickName;
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

    public void EnableRoomLoadingUI() {
        PV.RPC("RPC_EnableRoomLoadingUI", RpcTarget.All);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer) {
        base.OnPlayerLeftRoom(otherPlayer);
        Debug.Log(otherPlayer.NickName + " left the game");
        playersInRoom--;
        ClearPlayerListings();
        ListPlayers();
    }

    [PunRPC]
    void RPC_EnableRoomLoadingUI() {
        loadingTextObject.SetActive(true);
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
        PhotonNetwork.Instantiate(playerPrefab.name, Vector3.zero, Quaternion.identity);
        Outline[] allOutlines = FindObjectsOfType<Outline>();
        foreach (Outline allOLine in allOutlines) {
            allOLine.enabled = false;
        }
    }
}
