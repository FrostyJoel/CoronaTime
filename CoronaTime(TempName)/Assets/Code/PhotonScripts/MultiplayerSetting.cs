﻿using UnityEngine;

public class MultiplayerSetting : MonoBehaviour {
    public static MultiplayerSetting multiplayerSetting;

    public int maxPlayers, menuScene, multiplayerScene;

    private void Awake() {
        if (MultiplayerSetting.multiplayerSetting) {
            if(MultiplayerSetting.multiplayerSetting != this) {
                Destroy(this.gameObject);
            }
        } else {
            MultiplayerSetting.multiplayerSetting = this;
        }
        DontDestroyOnLoad(this.gameObject);
    }
}
