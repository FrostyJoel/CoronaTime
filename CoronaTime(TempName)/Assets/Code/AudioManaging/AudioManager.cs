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

    public static void PlaySound(AudioClip audioClipToPlay, Vector3 position, AudioGroups audioGroups) {
        AudioManager.PlaySound(audioClipToPlay, audioGroups, position);
    }

    public static void PlaySound(AudioClip audioClipToPlay, AudioGroups audioGroups, Vector3 position) {
        GameObject soundObject = new GameObject("Sound");
        soundObject.transform.position = position;
        AudioSource audio = soundObject.AddComponent<AudioSource>();
        audio.PlayOneShot(audioClipToPlay);
        audio.outputAudioMixerGroup = audioMixer.FindMatchingGroups(audioGroups.ToString())[0];
        GameObject.Destroy(soundObject, audioClipToPlay.length);
    }
}

