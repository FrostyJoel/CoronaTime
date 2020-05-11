using Photon.Pun;
using UnityEngine.UI;

public class ScriptPlayerListing : MonoBehaviourPun {
    public Text text_Nickname;//, text_PlayerReady;
    public InputField inputField;
    public Toggle toggle;
    public static ScriptPlayerListing splSingle;
    public bool ready;

    private void Awake() {
        splSingle = this;
    }

    public void SetReadyState(bool state) {
        inputField.text = state.ToString();
    }

    public void OnReadyStateChange(string state) {
        GetComponent<PhotonView>().RPC("RPC_ChangeReadyState", RpcTarget.All, state);
    }

    [PunRPC]
    void RPC_ChangeReadyState(string state) {
        inputField.text = state;
        ready = true;
    }
}
