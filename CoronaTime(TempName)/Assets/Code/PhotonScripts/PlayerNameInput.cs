using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using System;

public class PlayerNameInput : MonoBehaviour {
    public InputField nameInputField = null;
    public Button continueButton = null;

    private const string PlayerPrefsNameKey = "PlayerName";

    private void Start() {
        SetUpInputField();
    }

    private void SetUpInputField() {
        if (!PlayerPrefs.HasKey(PlayerPrefsNameKey)) { return; }
        string defaultName = PlayerPrefs.GetString(PlayerPrefsNameKey);
        nameInputField.text = defaultName;
        SetPlayerName(defaultName);
    }

    public void SetPlayerName(string name) {
        continueButton.interactable = !string.IsNullOrEmpty(name);
    }

    public void SavePlayerName() {
        string playerName = nameInputField.text;
        PhotonNetwork.NickName = playerName;
        PlayerPrefs.SetString(PlayerPrefsNameKey, playerName);
    }
}
