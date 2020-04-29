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
        roomGameObject.SetActive(true);
        myNumberInRoom = playersInRoom;   
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
                GameObject tempObject = Instantiate(playerListingPrefab, playersPanel);
                Text tempText = tempObject.transform.GetChild(0).GetComponent<Text>();
                tempText.text = PhotonNetwork.PlayerList[i].NickName;
            }
        }
    }

    public void StartGame() {
        isLoaded = true;
        PhotonNetwork.LoadLevel(currentScene + 1);
    }

    void OnSceneFinishedLoading(Scene scene, LoadSceneMode mode) {
        currentScene = scene.buildIndex;
        if(currentScene == MultiplayerSetting.multiplayerSetting.multiplayerScene) {
            isLoaded = true;
            RPC_CreatePlayer();
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
        pos.x += playersInGame;
        GameObject g = PhotonNetwork.Instantiate(playerPrefab.name, Vector3.zero, Quaternion.identity);
    }
}
