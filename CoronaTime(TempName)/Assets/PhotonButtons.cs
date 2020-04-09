using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PhotonButtons : MonoBehaviour
{
    public MenuLogic mLogic;
    public InputField createRoomInput, joinRoomInput;

    public void OnClickCreateRoom()
    {
        if(createRoomInput.text.Length >= 1)
        {
            PhotonNetwork.CreateRoom(createRoomInput.text, new RoomOptions() { MaxPlayers = 4 }, null);
        }
    }

    public void OnClickJoinRoom()
    {
        PhotonNetwork.JoinRoom(joinRoomInput.text);
    }

    private void OnJoinedRoom()
    {
        mLogic.DisableMenuUI();
        Debug.Log("We are connected to the room!");
    }
    
}
