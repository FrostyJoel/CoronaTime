using System.Collections;
using System.Collections.Generic;
using UnityEngine.Audio;
using UnityEngine;

public static class AudioManager {

    public static AudioMixer audioMixer;

    public enum AudioGroups {
        None,
        Music,
        SFX,
    }

    public static void PlaySound(AudioClip audioClipToPlay, AudioGroups audioGroups) {
        GameObject soundObject = new GameObject("Sound");
        AudioSource audio = soundObject.AddComponent<AudioSource>();
        audio.PlayOneShot(audioClipToPlay);
        soundObject.GetComponent<AudioSource>().outputAudioMixerGroup = audioMixer.FindMatchingGroups(audioGroups.ToString())[0];
        GameObject.Destroy(soundObject, audioClipToPlay.length);
    }
}

