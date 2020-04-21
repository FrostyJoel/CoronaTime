using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Options : MonoBehaviour
{
    public Toggle fullscreenToggle;
    // Start is called before the first frame update
    void Start()
    {
        fullscreenToggle.isOn = Screen.fullScreen;
    }

    public void SetFullscreen(bool fullscreenState)
    {
        Screen.fullScreen = fullscreenState; 
    } 
}
