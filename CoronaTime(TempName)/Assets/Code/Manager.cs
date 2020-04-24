using System.Collections;
using System.Collections.Generic;
using UnityEngine.Audio;
using UnityEngine.UI;
using System;
using Photon.Pun;
using UnityEngine;

public class Manager : MonoBehaviourPun {
    public StaticInformation staticInfo;
    public ColorManaging colorManaging;
    public static StaticInformation staticInformation;
    public static ColorManaging staticColorManaging;

    private void Awake() {
        staticInformation = staticInfo;

        for (int i = 0; i < colorManaging.amountColorOptions; i++) {
            colorManaging.colorPicks.Add(new ColorPick() {
                color = Color.HSVToRGB((float)i / colorManaging.amountColorOptions + 1f / colorManaging.amountColorOptions, 1, 1)
            });            
        }

        staticColorManaging = colorManaging;
        AudioManager.audioMixer = staticInformation.audioMixer;
    }
    private void Start()
    {
        Outline[] allOutlines = FindObjectsOfType<Outline>();
        foreach (Outline allOLine in allOutlines)
        {
            allOLine.enabled = false;
        }
        PhotonNetwork.SendRate = 10;
    }
}

[Serializable]
public class StaticInformation {
    public AudioMixer audioMixer;
}

[Serializable]
public class ColorManaging {
    public int amountColorOptions;
    public Sprite colorButtonSprite;
    public static List<float> rValue = new List<float>();
    public List<ColorPick> colorPicks = new List<ColorPick>();

    public static bool HasColorBeenUsed(Color color) {
        bool used = false;
        int index = WhereInList(color);
        if(index >= 0) {
            used = Manager.staticColorManaging.colorPicks[index].inUse;
        }
        return used;
    }

    static int WhereInList(Color color) {
        int index = -1;
        if (Manager.staticColorManaging.colorPicks.Count > 0) {
            for (int i = 0; i < Manager.staticColorManaging.colorPicks.Count; i++) {
                if (Manager.staticColorManaging.colorPicks[i].color == color) {
                    index = i;
                    break;
                }
            }
        }
        return index;
    }

    public static void UseColor(Material[] materials, Color color) {
        int index = WhereInList(color);
        if(index >= 0) {
            for (int i = 0; i < materials.Length; i++) {
                int oldIndex = WhereInList(materials[i].color);
                int newIndex = WhereInList(color);
                materials[i].color = color;
                Manager.staticColorManaging.colorPicks[newIndex].ChangeCurrentUseState(true);
                if(oldIndex >= 0) {
                    Manager.staticColorManaging.colorPicks[oldIndex].ChangeCurrentUseState(false);
                }
            }
        }
    }
}

[Serializable]
public class ColorPick {
    public Color color;
    public bool inUse;
    public List<Button> linkedColorButtons = new List<Button>();

    public void ChangeCurrentUseState(bool newUseState) {
        inUse = newUseState;
        if(linkedColorButtons.Count > 0) {
            for (int i = 0; i < linkedColorButtons.Count; i++) {
                if (linkedColorButtons[i]) {//incase of disconnect
                    linkedColorButtons[i].interactable = !newUseState;
                }
            }
        }
    }
}