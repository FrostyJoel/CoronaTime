using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PhotonButtons : MonoBehaviour
{
    public PhotonMenuHandler pMenuHandler;
    public InputField createRoomInput, joinRoomInput,playerNameInput;
    public Button createRoomButton, joinRoomButton;


    public void OnClickCreateRoom()
    {
        pMenuHandler.CreateNewRoom();
    }

    public void OnClickJoinRoom()
    {
        pMenuHandler.JoinOrCreateRoom();
    }

    public void CheckForCreateRoomName(string name)
    {
        createRoomButton.interactable = !string.IsNullOrEmpty(name);
    }

    public void CheckForJoinRoomName(string name)
    {
        joinRoomButton.interactable = !string.IsNullOrEmpty(name);
    }
}
