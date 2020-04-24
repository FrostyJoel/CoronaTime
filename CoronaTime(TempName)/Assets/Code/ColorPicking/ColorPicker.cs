using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class ColorPicker : MonoBehaviourPun {

    public Controller controller;
    public Material[] targetMaterials;
    List<Material> renderMats = new List<Material>();
    public Transform buttonParent;
    public Button continueButton;
    public Color pickedColor;
    public bool pickedAColor;

    private void Awake() {
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    void Start() {
        if (photonView.IsMine || controller.playerView.devView) {
            continueButton.interactable = false;
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

            if (FindObjectOfType(typeof(Manager))) {
                buttonParent.gameObject.SetActive(true);
                for (int i = 0; i < Manager.staticColorManaging.amountColorOptions; i++) {
                    GameObject colorButton = new GameObject("Color Option " + i);
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
            } else {
                buttonParent.gameObject.SetActive(false);
            }
        }
    }

    public void SaveColorAndContinue() {
        if (photonView.IsMine || controller.playerView.devView) {
            pickedAColor = true;
        }
    }

    void SetColor(Color color) {
        if (photonView.IsMine || controller.playerView.devView) {
            continueButton.interactable = true;
            if (!ColorManaging.HasColorBeenUsed(color)) {
                if (renderMats.Count > 0) {
                    ColorManaging.UseColor(renderMats.ToArray(), color);
                    pickedColor = color;
                }
            }
        }
    }
}