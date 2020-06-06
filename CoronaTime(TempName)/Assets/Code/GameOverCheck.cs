﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using Photon.Pun;

public class GameOverCheck : MonoBehaviourPun {
    public static GameOverCheck goc_Single;
    public GameObject go_VictoryScreen;
    public VictoryScreenListing[] victoryScreenSlots;

    public CartStorage[] storages;

    private void Awake() {
        goc_Single = this;
        go_VictoryScreen.SetActive(false);
        for (int i = 0; i < victoryScreenSlots.Length; i++) {
            victoryScreenSlots[i].text_Name.text = "";
            victoryScreenSlots[i].text_Score.text = "";
        }
    }

    public void GameOver() {
        CheckScore();
    }

    public void CheckScore() {
        storages = FindObjectsOfType<CartStorage>();
        List<int> scores = new List<int>();
        for (int i = 0; i < storages.Length; i++) {
            scores.Add(storages[i].score);
        }
        scores.Sort();
        scores.Reverse();
        for (int i = 0; i < scores.Count; i++) {
            for (int iB = 0; iB < storages.Length; iB++) {
                if (storages[iB].score == scores[i]) {
                    victoryScreenSlots[i].text_Name.text = PhotonRoomCustomMatchMaking.roomSingle.RemoveIdFromNickname(storages[iB].photonView.Owner.NickName);
                    victoryScreenSlots[i].text_Score.text = scores[i].ToString();
                    break;
                }
            }
        }
        photonView.RPC("RPC_EndGame", RpcTarget.All);
    }

    [PunRPC]
    void RPC_EndGame() {
        go_VictoryScreen.SetActive(true);
        Controller[] controllers = FindObjectsOfType<Controller>();
        for (int i = 0; i < controllers.Length; i++) {
            controllers[i].canMove = false;
            controllers[i].localInGameHud.SetActive(false);
        }
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}

[System.Serializable]
public class VictoryScreenListing {
    public Text text_Name;
    public Text text_Score;
}