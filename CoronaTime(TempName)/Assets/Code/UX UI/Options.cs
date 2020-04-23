using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Options : MonoBehaviour
{
    public Toggle fullscreenToggle;
    public Dropdown resolutionsDropdown;
    Resolution[] resolutions;
    public Slider[] masterSlider, musicSlider, sfxSlider;

    delegate void onValueChanged(float value);

    // Start is called before the first frame update
    void Start()
    {
        ApplyMasterListener();
        ApplyMusicListener();
        ApplySFXListener();
        AssignVolumeSliderArrayValue(masterSlider, "MasterVolume", -30);
        AssignVolumeSliderArrayValue(musicSlider, "MusicVolume", -30);
        AssignVolumeSliderArrayValue(sfxSlider, "SFXVolume", -30);
        int index = 0;
        resolutions = Screen.resolutions;
        List<string> resolutionStringList = new List<string>();
        List<Resolution> tempResolutions = new List<Resolution>();        

        for(int i = resolutions.Length-1; i > 0; i--)
        {
            tempResolutions.Add(resolutions[i]);
            string option = resolutions[i].width + " x " + resolutions[i].height;
            resolutionStringList.Add(option);
            if (resolutions[i].width == Screen.width && resolutions[i].height == Screen.height)
            {
                index = i;
            }
        }

        resolutions = tempResolutions.ToArray();
        fullscreenToggle.isOn = Screen.fullScreen;
        resolutionsDropdown.ClearOptions();
        resolutionsDropdown.AddOptions(resolutionStringList);
        resolutionsDropdown.value = index;
        resolutionsDropdown.RefreshShownValue();
        //print(resolutions[resolutions.Length-1].height + " x " + resolutions[resolutions.Length-1].width);
    }

    public void OnResolutionDropdownChange(int index)
    {
        Screen.SetResolution(resolutions[index].width, resolutions[index].height, Screen.fullScreen);
    }

    public void SetFullscreen(bool fullscreenState)
    {
        Screen.fullScreen = fullscreenState; 
    }

    public void Quit()
    {
        Application.Quit();
    }

    void ApplyMasterListener() {
        if(masterSlider.Length > 0) {
            for (int i = 0; i < masterSlider.Length; i++) {
                masterSlider[i].onValueChanged.RemoveAllListeners();
                masterSlider[i].onValueChanged.AddListener(OnMasterVolumeChange);
            }
        }
    }
    
    void ApplyMusicListener() {
        if(musicSlider.Length > 0) {
            for (int i = 0; i < musicSlider.Length; i++) {
                musicSlider[i].onValueChanged.RemoveAllListeners();
                musicSlider[i].onValueChanged.AddListener(OnMusicVolumeChange);
            }
        }
    }
    
    void ApplySFXListener() {
        if(sfxSlider.Length > 0) {
            for (int i = 0; i < sfxSlider.Length; i++) {
                sfxSlider[i].onValueChanged.RemoveAllListeners();
                sfxSlider[i].onValueChanged.AddListener(OnSFXVolumeChange);
            }
        }
    }

    public void OnMasterVolumeChange(float value)
    {
        AudioManager.audioMixer.SetFloat("MasterVolume", value);
        PlayerPrefs.SetFloat("MasterVolume", value);
    }

    public void OnMusicVolumeChange(float value)
    {
        AudioManager.audioMixer.SetFloat("MusicVolume", value);
        PlayerPrefs.SetFloat("MusicVolume", value);
    }

    public void OnSFXVolumeChange(float value)
    {
        AudioManager.audioMixer.SetFloat("SFXVolume", value);
        PlayerPrefs.SetFloat("SFXVolume", value);
    }

    public void AssignVolumeSliderArrayValue(Slider[] sliders, string prefName, float fallBackValue)
    {
        if (sliders.Length > 0)
        {
            for (int i = 0; i < sliders.Length; i++)
            {
                sliders[i].value = PlayerPrefs.GetFloat(prefName, fallBackValue);
            }
        }
    }
}
