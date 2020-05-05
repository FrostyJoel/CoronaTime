using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using Photon.Realtime;
using UnityEngine;

public class Spawnpoints : MonoBehaviourPunCallbacks {
    public static Spawnpoints spawnpointsSingleton;
    public List<Spawnpoint> spawnpoints = new List<Spawnpoint>();
    public int players = 0;
    public List<PlayerView> playerViews = new List<PlayerView>();
    private void Awake() {
        spawnpointsSingleton = this;
    }

    private void Update() {
        if(playerViews.Count > 1) {
            SetStartPositions();
        }
    }

    public void SetStartPositions() {
        CheckPlayerViews();
        if(playerViews.Count > 0) {
            if (spawnpoints.Count > 0) {
                for (int i = 0; i < playerViews.Count; i++) {
                    playerViews[i].transform.position = spawnpoints[i].actualSpawnpoint.position;
                }
            } else {
                Debug.Log("No Spawnpoints");
            }
        } else {
            Debug.Log("No Playerviews");
        }
    }

    void CheckPlayerViews() {
        if (playerViews.Count == 0) {
            PlayerView[] views = FindObjectsOfType<PlayerView>();
            for (int i = 0; i < views.Length; i++) {
                bool added = false;
                if (playerViews.Count > 0) {
                    for (int iB = 0; iB < playerViews.Count; iB++) {
                        if (views[i].myView.ViewID < playerViews[iB].myView.ViewID) {
                            AddToListAtIndex(views[i], iB);
                            added = true;
                            break;
                        }
                    }
                    if (!added) {
                        playerViews.Add(views[i]);
                    }
                } else {
                    playerViews.Add(views[i]);
                }
            }
        }
    }

    void AddToListAtIndex(PlayerView view, int index) {
        playerViews.Add(new PlayerView());
        for (int i = playerViews.Count - 2; i >= index; i--) {
            playerViews[i + 1] = playerViews[i];
        }
        playerViews[index] = view;
    }
}