using Photon.Pun;
using UnityEngine.UI;

public class ScriptPlayerListing : MonoBehaviourPun {
    public Text text_Nickname, text_PlayerReady;
    public Toggle toggle;
    public void SetReadyState(bool state) {
        text_PlayerReady.text = state.ToString();
        //this.photonView.RPC("RPC_ChangeNickname", RpcTarget.All, state);
    }

    [PunRPC]
    void RPC_ChangeReadyState(string nickname) {
        text_Nickname.text = nickname;
    }
}
