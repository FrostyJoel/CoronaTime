using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhotonConnect : MonoBehaviour
{
    public string versionName = "0.1";

    public GameObject selectionview, selectionview1, selectionview2;

    private void Awake()
    {
        PhotonNetwork.ConnectUsingSettings(versionName);
        Debug.Log("Connecting to version: " + versionName);
    }

    private void OnConnectedToMaster()
    {
        selectionview.SetActive(false);
        selectionview1.SetActive(true);
        PhotonNetwork.JoinLobby(TypedLobby.Default);

        Debug.Log("We are connected to the master");
    }

    private void OnJoinedLobby()
    {
        Debug.Log("On Joined Lobby");
    }

    private void OnDisconnectedFromPhoton()
    {
        if (selectionview.activeSelf)
        {
            selectionview.SetActive(false);
        }

        if (selectionview1.activeSelf)
        {
            selectionview1.SetActive(false);
        }

        selectionview2.SetActive(true);

        Debug.Log("Disconnected from photon services");
    }

}
