using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PhotonButtons : MonoBehaviour
{
    public PhotonMenuHandler pMenuHandler;
    public InputField createRoomInput, joinRoomInput;

    public void OnClickCreateRoom()
    {
        pMenuHandler.CreateNewRoom();
    }

    public void OnClickJoinRoom()
    {
        pMenuHandler.JoinOrCreateRoom();
    }
}
