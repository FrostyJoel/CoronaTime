////////////////////////////////////////////////////////
//Steam has to be running in background for this to work
////////////////////////////////////////////////////////
using Steamworks;
using UnityEngine;

public class SteamTest : MonoBehaviour {

    public void Awake() {
        DontDestroyOnLoad(gameObject);
    }

    private void Start() {
        if(!SteamManager.Initialized) { return; }
        string myName = SteamFriends.GetPersonaName();
        int friendCount = SteamFriends.GetFriendCount(EFriendFlags.k_EFriendFlagImmediate);
        for (int i = 0; i < friendCount; i++) {
            CSteamID friend = SteamFriends.GetFriendByIndex(i, EFriendFlags.k_EFriendFlagImmediate);
            string friendName = SteamFriends.GetFriendPersonaName(friend);
            print(myName + " is friends with " + friendName);
        }
    }
}
