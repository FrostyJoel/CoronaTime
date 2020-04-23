using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Options : MonoBehaviour
{
    public Toggle fullscreenToggle;
    public Dropdown resolutionsDropdown;
    Resolution[] resolutions;
    public Slider[] masterSlider, sfxSlider, musicSlider;
    public Slider testSlider;


    // Start is called before the first frame update
    void Start()
    {
        AssignVolumeSliderArrayValue(masterSlider, "MasterVolume", -30);
        AssignVolumeSliderArrayValue(musicSlider, "MusicVolume", -30);
        AssignVolumeSliderArrayValue(sfxSlider, "SFXVolume", -30);
        int index = 0;
        resolutions = Screen.resolutions;
        List<string> resolutionStringList = new List<string>();
        List<Resolution> tempResolutions = new List<Resolution>();

        testSlider.onValueChanged.RemoveAllListeners();
        testSlider.onValueChanged.AddListener(OnSliderValueChange);
        

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
    public delegate void test(float f);
    public void OnSliderValueChange(float sliderValue, test Test)
    {
        Test(sliderValue);
    }

    public void TestPrint(float value)
    {
        print("yes" + value); 
    }

    public void AssignVolumeSliderArrayValue(Slider[] slider, string prefName, float fallBackValue)
    {
        if (slider.Length > 0)
        {
            for (int i = 0; i < slider.Length; i++)
            {
                slider[i].value = PlayerPrefs.GetFloat(prefName, fallBackValue);
            }
        }
    }
}
