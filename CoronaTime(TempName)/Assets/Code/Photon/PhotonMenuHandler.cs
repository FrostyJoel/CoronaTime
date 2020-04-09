using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PhotonMenuHandler : MonoBehaviour
{
    public PhotonButtons photonB;

    public GameObject mainPlayer;
    public List<GameObject> spawnPoints = new List<GameObject>();

    private void Awake()
    {
        DontDestroyOnLoad(transform);

        SceneManager.sceneLoaded += OnSceneFinishedLoading;
    }

    public void MoveScene()
    {
        PhotonNetwork.LoadLevel("Scene_Joël");
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
        if (scene.name == "Scene_Joël")
        {
            SpawnPlayer();
        }
    }

    private void SpawnPlayer()
    {
        GameObject[] spawnLocations = GameObject.FindGameObjectsWithTag("SpawnLocaties");
        Debug.Log(spawnLocations.Length);
        for (int i = 0; i < spawnLocations.Length; i++)
        {
            if (!spawnPoints.Contains(spawnLocations[i]))
            {
                spawnPoints.Add(spawnLocations[i]);
                PhotonNetwork.Instantiate(mainPlayer.name, spawnPoints[i].transform.position, mainPlayer.transform.rotation, 0);
                break;
            }
        }
    }

}
