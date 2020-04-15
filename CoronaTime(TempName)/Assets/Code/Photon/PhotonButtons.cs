using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PhotonButtons : MonoBehaviour
{
    public PhotonMenuHandler pMenuHandler;
    public InputField createRoomInput, joinRoomInput, playerNameInput;
    public Button createRoomButton, joinRoomButton;


    public void OnClickCreateRoom()
    {
        pMenuHandler.CreateNewRoom();
    }

    public void OnClickJoinRoom()
    {
        pMenuHandler.JoinOrCreateRoom();
    }

    public void CheckForCreateRoomName(string name) {
        bool state = false;

        if (!string.IsNullOrEmpty(name)) {
            if(!name.Contains(" ")) {
                state = true;
            }
        }
        createRoomButton.interactable = state;
    }

    public void CheckForJoinRoomName(string name) {
        bool state = false;

        if (!string.IsNullOrEmpty(name)) {
            if (!name.Contains(" ")) {
                state = true;
            }
        }
        joinRoomButton.interactable = state;
    }
}
