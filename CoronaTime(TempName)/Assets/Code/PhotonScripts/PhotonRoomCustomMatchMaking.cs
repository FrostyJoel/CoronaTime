using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PhotonRoomCustomMatchMaking : MonoBehaviourPunCallbacks, IInRoomCallbacks {
    public static PhotonRoomCustomMatchMaking roomSingle;
    public GameObject playerPrefab, lobbyGameObject, roomGameObject, playerListingPrefab, startButton, text_Loading;
    public Transform playersPanel;
    [HideInInspector] public PhotonView PV;

    public bool isLoaded;
    public int currentScene;
    [Space]
    Player[] photonPlayers;
    public int playersInRoom, myNumberInRoom;
    [Space]
    public int playersInGame;
    public bool dev;
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
        Debug.LogWarning("[start]" + GetType() + " " + System.Reflection.MethodInfo.GetCurrentMethod());
        PhotonNetwork.AddCallbackTarget(this);
        SceneManager.sceneLoaded += OnSceneFinishedLoading;
        Debug.LogWarning("[end]" + GetType() + " " + System.Reflection.MethodInfo.GetCurrentMethod());
    }

    public override void OnDisable() {
        Debug.LogWarning("[start]" + GetType() + " " + System.Reflection.MethodInfo.GetCurrentMethod());
        PhotonNetwork.RemoveCallbackTarget(this);
        SceneManager.sceneLoaded -= OnSceneFinishedLoading;
        Debug.LogWarning("[end]" + GetType() + " " + System.Reflection.MethodInfo.GetCurrentMethod());
    }

    private void Start() {
        PV = GetComponent<PhotonView>();
    }

    public override void OnJoinedRoom() {
        Debug.LogWarning("[start]" + GetType() + " " + System.Reflection.MethodInfo.GetCurrentMethod());
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
        if (startButton && dev) {
            if (PhotonNetwork.IsMasterClient) {
                startButton.SetActive(true);
            } else {
                startButton.SetActive(false);
            }
        }
        Debug.LogWarning("[end]" + GetType() + " " + System.Reflection.MethodInfo.GetCurrentMethod());
    }

    public override void OnPlayerEnteredRoom(Player newPlayer) {
        Debug.LogWarning("[start]" + GetType() + " " + System.Reflection.MethodInfo.GetCurrentMethod());
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
        Debug.LogWarning("[end]" + GetType() + " " + System.Reflection.MethodInfo.GetCurrentMethod());
    }

    void ClearPlayerListings() {
        Debug.LogWarning("[start]" + GetType() + " " + System.Reflection.MethodInfo.GetCurrentMethod());
        if (playersPanel) {
            for (int i = playersPanel.childCount - 1; i >= 0; i--) {
                Destroy(playersPanel.GetChild(i).gameObject);
            }
        }
        Debug.LogWarning("[end]" + GetType() + " " + System.Reflection.MethodInfo.GetCurrentMethod());
    }

    void ListPlayers() {
        Debug.LogWarning("[start]" + GetType() + " " + System.Reflection.MethodInfo.GetCurrentMethod());
        if (PhotonNetwork.InRoom) {
            for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++) {
                GameObject tempNickNameObject = Instantiate(playerListingPrefab, playersPanel);
                ScriptPlayerListing spl = tempNickNameObject.GetComponent<ScriptPlayerListing>();
                string nickname = RemoveIdFromNickname(PhotonNetwork.PlayerList[i].NickName);
                spl.text_Nickname.text = nickname;
            }
        }
        Debug.LogWarning("[end]" + GetType() + " " + System.Reflection.MethodInfo.GetCurrentMethod());
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

    public override void OnPlayerLeftRoom(Player otherPlayer) {
        Debug.LogWarning("[start]" + GetType() + " " + System.Reflection.MethodInfo.GetCurrentMethod());
        base.OnPlayerLeftRoom(otherPlayer);
        Debug.Log(otherPlayer.NickName + " left the game");
        playersInRoom--;
        ClearPlayerListings();
        ListPlayers();
        Debug.LogWarning("[end]" + GetType() + " " + System.Reflection.MethodInfo.GetCurrentMethod());
    }

    public void EnableRoomLoadingUI() {
        PV.RPC("RPC_EnableRoomLoadingUI", RpcTarget.All);
    }

    [PunRPC]
    void RPC_EnableRoomLoadingUI() {
        if (text_Loading) {
            text_Loading.SetActive(true);
        }
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
