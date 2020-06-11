using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiplayerSetting : MonoBehaviour {
    public static MultiplayerSetting multiplayerSetting;

    public int maxPlayers, menuScene, multiplayerScene;

    private void Awake() {
        Debug.LogWarning("[start]" + GetType() + " " + System.Reflection.MethodInfo.GetCurrentMethod());
        if (MultiplayerSetting.multiplayerSetting) {
            if(MultiplayerSetting.multiplayerSetting != this) {
                Destroy(this.gameObject);
            }
        } else {
            MultiplayerSetting.multiplayerSetting = this;
        }
        DontDestroyOnLoad(this.gameObject);
        Debug.LogWarning("[end]" + GetType() + " " + System.Reflection.MethodInfo.GetCurrentMethod());
    }
}
