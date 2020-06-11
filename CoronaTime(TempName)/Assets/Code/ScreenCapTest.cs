using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenCapTest : MonoBehaviour {
    int i;
    public bool ss;
    private void Start() {
        Debug.LogWarning(Application.dataPath);
    }

    private void Update() {
        if (ss) {
            ScreenCapture.CaptureScreenshot("Assets/ScreenShotTest"+i+".png");
            ss = false;
            i++;
        }
    }
}
