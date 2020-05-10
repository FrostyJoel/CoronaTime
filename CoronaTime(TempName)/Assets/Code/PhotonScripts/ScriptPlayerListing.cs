using Photon.Pun;
using UnityEngine.UI;

public class ScriptPlayerListing : MonoBehaviourPun {
    public Text text_Nickname, text_PlayerReady;
    public Toggle toggle;
    public static ScriptPlayerListing splSingle;

    private void Awake() {
        splSingle = this;
    }

    public void SetReadyState(bool state) {
        text_PlayerReady.text = state.ToString();
        //PhotonRoomCustomMatchMaking.room.PV.RPC("RPC_ChangeReadyState", RpcTarget.All, state);
    }

    [PunRPC]
    void RPC_ChangeReadyState(bool state) {
        text_PlayerReady.text = state.ToString();
    }
}
