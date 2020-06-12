using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class UiElementSounds : MonoBehaviour {
    public List<UiElement> element = new List<UiElement>();
    private void Start() {
        if (element.Count > 0) {
            //for (int i = 0; i < element.Count; i++) {
            //    element[i].AddSoundListener(gameObject);
            //}
            foreach(UiElement uiElement in element)
            {
                uiElement.AddSoundListener(gameObject);
                uiElement.eus = this;
            }
        }
    }
}

[System.Serializable]
public class UiElement {
    [HideInInspector] public UiElementSounds eus;
    public enum type {
        button,
        toggle,
        slider,
        dropdown
    }
    public type ThisType;
    public AudioClip audioClip;

    public AudioManager.AudioGroups audioGroup;

    [Header("Ui Elements")]
    public Button button;
    public Toggle toggle;
    public Slider slider;
    public Dropdown dropdown;

    public void AddSoundListener(GameObject elementObject) {
        switch (ThisType) {
            case type.button:
            if (!button && elementObject.GetComponent<Button>()) {
                button = elementObject.GetComponent<Button>();
            }
            if (button) {
                if (audioClip) {
                    button.onClick.AddListener(() => AudioManager.PlaySound(audioClip, eus.transform.position, audioGroup));
                }
            } 
            break;
            case type.toggle:
            if (!toggle && elementObject.GetComponent<Toggle>()) {
                toggle = elementObject.GetComponent<Toggle>();
            }
            if (toggle) {
                toggle.onValueChanged.AddListener(OnToggleChange);
            }
            break;
            case type.slider:
            if(!slider && elementObject.GetComponent<Slider>()) {
                slider = elementObject.GetComponent<Slider>();
            }
            if (slider) {
                SliderEvent se = slider.gameObject.AddComponent<SliderEvent>();
                se.uiElement = this;
            }
            break;
            case type.dropdown:
            if(!dropdown && elementObject.GetComponent<Dropdown>()) {
                dropdown = elementObject.GetComponent<Dropdown>();
            }
            if (dropdown) {
                dropdown.onValueChanged.AddListener(OnDropDownChanged);
            }
            break;
        }
    }

    void OnToggleChange(bool b) {
        AudioManager.PlaySound(audioClip, eus.transform.position, audioGroup);
    }

    public void OnDropDownChanged(int i) {
        AudioManager.PlaySound(audioClip, eus.transform.position, audioGroup);
    }
}