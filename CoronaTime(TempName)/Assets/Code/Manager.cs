using System.Collections;
using System.Collections.Generic;
using UnityEngine.Audio;
using UnityEngine;

public class Manager : MonoBehaviour {
    public StaticInformation staticInfo;
    public static StaticInformation staticInformation;

    private void Awake() {
        staticInformation = staticInfo;
        AudioManager.audioMixer = staticInformation.audioMixer;
    }
}

[System.Serializable]
public class StaticInformation {
    public AudioMixer audioMixer;
}
