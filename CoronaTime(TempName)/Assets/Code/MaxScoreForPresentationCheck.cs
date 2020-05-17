using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using Photon.Pun;

public class MaxScoreForPresentationCheck : MonoBehaviourPun {
    public static MaxScoreForPresentationCheck maxScoreFpsSingle;
    public int int_MaxRequiredScore;
    public GameObject go_VictoryScreen;
    public VictoryScreenListing[] victoryScreenSlots;

    public List<CartStorage> lstorages = new List<CartStorage>();

    private void Awake() {
        maxScoreFpsSingle = this;
        go_VictoryScreen.SetActive(false);
        for (int i = 0; i < victoryScreenSlots.Length; i++) {
            victoryScreenSlots[i].text_Name.text = "";
            victoryScreenSlots[i].text_Score.text = "";
        }
    }

    public void CheckScore(int id) {
        CartStorage[] storages = FindObjectsOfType<CartStorage>();
        if (PhotonNetwork.GetPhotonView(id).Owner.IsLocal) {
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
            if (scores[0] >= int_MaxRequiredScore) {
                photonView.RPC("RPC_EndGame", RpcTarget.All);
            }
        }
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

    public void SetStoragesList(CartStorage[] array_Storages) {
        for (int i = 0; i < array_Storages.Length; i++) {
            lstorages.Add(array_Storages[i]);
        }
    }
}

[System.Serializable]
public class VictoryScreenListing {
    public Text text_Name;
    public Text text_Score;
}