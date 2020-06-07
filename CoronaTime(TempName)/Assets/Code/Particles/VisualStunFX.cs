using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using Photon.Pun;

public class VisualStunFX : VisualFX {

    private void Start() {
        anim.gameObject.SetActive(false);
        if (GetComponentInParent<PhotonView>().Owner.IsLocal) {
            for (int i = 0; i < ps.Length; i++) {
                ps[i].gameObject.layer = 10;
            }
        }
    }

    public override void StartStopVisualFX(bool play) {
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
        if (anim) {
            if (!play) {
                anim.gameObject.SetActive(true);
                Invoke("DisableOverTime", ps[0].main.duration);
            }
        }
    }

    void DisableOverTime() {
        anim.gameObject.SetActive(false);
    }
}
