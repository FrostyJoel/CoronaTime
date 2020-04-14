using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PhotonMenuHandler : MonoBehaviour
{
    public PhotonButtons photonB;
    public string sceneName;
    public GameObject mainPlayer;
    public List<GameObject> spawnPoints = new List<GameObject>();

    private void Awake()
    {
        DontDestroyOnLoad(transform);

        SceneManager.sceneLoaded += OnSceneFinishedLoading;
    }

    public void MoveScene()
    {
        PhotonNetwork.LoadLevel(sceneName);
    }

    public void CreateNewRoom()
    {
        PhotonNetwork.CreateRoom(photonB.createRoomInput.text, new RoomOptions() { MaxPlayers = 4 }, null);
    }

    public void JoinOrCreateRoom()
    {
        RoomOptions roomOptions = new RoomOptions()
        {
            MaxPlayers = 4
        };
        PhotonNetwork.JoinOrCreateRoom(photonB.joinRoomInput.text, roomOptions, TypedLobby.Default);
    }

    private void OnJoinedRoom()
    {
        MoveScene();
        Debug.Log("We are connected to the room!");
    }

    private void OnSceneFinishedLoading(Scene scene,LoadSceneMode mode)
    {
        if (scene.name == sceneName)
        {
            SpawnPlayer();
        }
    }

    private void SpawnPlayer()
    {
        spawnPoints = new List<GameObject>(GameObject.FindGameObjectsWithTag("SpawnLocaties"));
        Debug.Log(spawnPoints.Count);
        for (int i = 0; i < spawnPoints.Count; i++)
        {
            if (spawnPoints[i].activeSelf)
            {
                GameObject player = PhotonNetwork.Instantiate(mainPlayer.name, spawnPoints[i].transform.position, mainPlayer.transform.rotation, 0);
                if(photonB.playerNameInput.text.Length > 0)
                {
                    PhotonNetwork.playerName = photonB.playerNameInput.text;
                }
                else
                {
                    PhotonNetwork.playerName = "Guest";
                }
                player.GetComponent<PlayerviewCheck>().plNameText.text = PhotonNetwork.playerName;
                spawnPoints[i].SetActive(false);
                spawnPoints.Remove(spawnPoints[i]);
                break;
            }
        }
    }

}
