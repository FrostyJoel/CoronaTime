using UnityEngine;
using System;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class ColorPicker : MonoBehaviour {

    public Material[] targetMaterials;
    List<Material> renderMats = new List<Material>();
    public Transform buttonParent;

    void Start() {
        Renderer[] rends = transform.GetComponentsInChildren<Renderer>();

        if (targetMaterials.Length > 0 && rends.Length > 0) {
            for (int i = 0; i < rends.Length; i++) {
                for (int iB = 0; iB < rends[i].materials.Length; iB++) {
                    Material[] mats = rends[i].materials;
                    for (int iC = 0; iC < targetMaterials.Length; iC++) {
                        if (mats[iB] && targetMaterials[iC] && mats[iB].name == targetMaterials[iC].name + " (Instance)") {
                            renderMats.Add(rends[i].materials[iB]);
                        }
                    }
                }
            }
        }

        for (int i = 0; i < Manager.staticColorManaging.amountColorOptions; i++) {
            GameObject colorButton = Instantiate(new GameObject("Color Option " + i));
            Image img = colorButton.AddComponent<Image>();
            img.sprite = Manager.staticColorManaging.colorButtonSprite;
            Color color = Manager.staticColorManaging.colorPicks[i].color;
            img.color = color;
            Button button = colorButton.AddComponent<Button>();
            button.onClick.AddListener(() => SetColor(color));
            button.interactable = !ColorManaging.HasColorBeenUsed(img.color);
            colorButton.transform.SetParent(buttonParent);
            Manager.staticColorManaging.colorPicks[i].linkedColorButtons.Add(button);
        }    

        SetColor(Color.white);
    }

    public void SetColor(Color color) {
        if (!ColorManaging.HasColorBeenUsed(color)) {
            if (renderMats.Count > 0) {
                ColorManaging.UseColor(renderMats.ToArray(), color);
            }
        }
    }
}