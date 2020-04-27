using Photon.Pun;
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
    PhotonView PV;

    public bool isLoaded;
    public int currentScene;
    [Space]
    Player[] photonPlayers;
    public int playersInRoom, myNumberInRoom;
    [Space]
    public int playersInGame;

    public bool readyToCount, readyToStart;
    public float startingTime;
    float lessThanMaxPlayers, atMaxPlayers, timeToStart;

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

    private void Update() {
        if (MultiplayerSetting.multiplayerSetting.delayStart) {
            if(playersInRoom == 1) {
                RestartTimer();
            }
            if (!isLoaded) {
                if (readyToStart) {
                    atMaxPlayers -= Time.deltaTime;
                    lessThanMaxPlayers = atMaxPlayers;
                    timeToStart = atMaxPlayers;
                } else if (readyToCount) {
                    lessThanMaxPlayers -= Time.deltaTime;
                    timeToStart = lessThanMaxPlayers;
                }
                if (timeToStart <= 0) {
                    StartGame();
                }
            }
        }
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
        readyToCount = false;
        readyToStart = false;
        lessThanMaxPlayers = startingTime;
        atMaxPlayers = 6;
        timeToStart = startingTime;
    }

    public override void OnJoinedRoom() {
        base.OnJoinedRoom();
        Debug.Log("Joined room");
        photonPlayers = PhotonNetwork.PlayerList;
        playersInRoom = photonPlayers.Length;
        CLearPlayerListings();
        ListPlayers();
        lobbyGameObject.SetActive(false);
        roomGameObject.SetActive(true);
        myNumberInRoom = playersInRoom;
        if (MultiplayerSetting.multiplayerSetting.delayStart) {
            if (playersInRoom > 1) {
                readyToCount = true;
            }
            if (playersInRoom == MultiplayerSetting.multiplayerSetting.maxPlayers) {
                readyToStart = true;
                if (PhotonNetwork.IsMasterClient) {
                    PhotonNetwork.CurrentRoom.IsOpen = false;
                }
            }
        }
        //} else {
        //    StartGame();
        //}
    }

    public override void OnPlayerEnteredRoom(Player newPlayer) {
        base.OnPlayerEnteredRoom(newPlayer);
        Debug.Log("player joined the room");
        CLearPlayerListings();
        ListPlayers();
        lobbyGameObject.SetActive(false);
        roomGameObject.SetActive(true);
        if (PhotonNetwork.IsMasterClient) {
            startButton.SetActive(true);
        }

        CLearPlayerListings();
        ListPlayers();

        playersInRoom++;
        if (MultiplayerSetting.multiplayerSetting.delayStart) {
            if(playersInRoom > 1) {
                readyToCount = true;
            }
            if(playersInRoom == MultiplayerSetting.multiplayerSetting.maxPlayers) {
                readyToStart = true;
                if (PhotonNetwork.IsMasterClient) {
                    PhotonNetwork.CurrentRoom.IsOpen = false;
                }
            }
        }
    }

    void CLearPlayerListings() {
        for (int i = playersPanel.childCount - 1; i >= 0; i--) {
            Destroy(playersPanel.GetChild(i).gameObject);
        }
    }

    void ListPlayers() {
        if (PhotonNetwork.InRoom) {
            for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++) {
                GameObject tempObject = Instantiate(playerListingPrefab, playersPanel);
                Text tempText = tempObject.transform.GetChild(0).GetComponent<Text>();
                tempText.text = PhotonNetwork.PlayerList[i].NickName;
            }
        }
    }

    public void StartGame() {
        isLoaded = true;
        if (PhotonNetwork.IsMasterClient) {
            if (MultiplayerSetting.multiplayerSetting.delayStart) {
                PhotonNetwork.CurrentRoom.IsOpen = false;
            }
        }
        PhotonNetwork.LoadLevel(MultiplayerSetting.multiplayerSetting.multiplayerScene);
    }

    void RestartTimer() {
        lessThanMaxPlayers = startingTime;
        timeToStart = startingTime;
        atMaxPlayers = 6;
        readyToStart = false;
        readyToCount = false;
    }

    void OnSceneFinishedLoading(Scene scene, LoadSceneMode mode) {
        currentScene = scene.buildIndex;
        if(currentScene == MultiplayerSetting.multiplayerSetting.multiplayerScene) {
            isLoaded = true;
            if (MultiplayerSetting.multiplayerSetting.delayStart) {
                PV.RPC("RPC_LoadedGameScene", RpcTarget.MasterClient);
            } else {
                RPC_CreatePlayer();
            }
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
        PhotonNetwork.Instantiate(playerPrefab.name, Vector3.zero, Quaternion.identity);
    }
}
