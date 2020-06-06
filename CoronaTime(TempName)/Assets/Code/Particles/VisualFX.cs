using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisualFX : MonoBehaviour { 

    ParticleSystem[] ps;
    public GameObject[] fxObjects;
    public Animator anim;

    void Start() {
        ps = GetComponentsInChildren<ParticleSystem>();
        if (anim) {
            anim.enabled = false;
        }
    }

    public virtual void StartStopVisualFX(bool play) {
        print(play);
        if (fxObjects.Length > 0) {
            for (int i = 0; i < fxObjects.Length; i++) {
                fxObjects[i].SetActive(play);
                fxObjects[i].SetActive(play);
            }
            if (anim) {
                anim.enabled = play;
            }
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
}