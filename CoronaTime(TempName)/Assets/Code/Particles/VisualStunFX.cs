using UnityEngine;
using Photon.Pun;

public class VisualStunFX : VisualFX {

    public float fadeTime;

    public override void Start() {
        base.Start();
        anim.enabled = false;
        anim.gameObject.SetActive(false);
        if (GetComponentInParent<PhotonView>().Owner.IsLocal) {
            for (int i = 0; i < ps.Length; i++) {
                ps[i].gameObject.layer = 10;
            }
        }
    }

    public override void StartStopVisualFX(bool play, Vector3 pos, bool posIsLocal) {
        for (int i = 0; i < fxObjects.Length; i++) {
            fxObjects[i].SetActive(play);
        }
        if (ps.Length > 0) {
            for (int i = 0; i < ps.Length; i++) {
                if (play) {
                    ps[i].Play();
                } else {
                    ps[i].Stop();
                }
            }
        }
    }

    void DisableOverTime() {
        anim.gameObject.SetActive(false);
    }
}
