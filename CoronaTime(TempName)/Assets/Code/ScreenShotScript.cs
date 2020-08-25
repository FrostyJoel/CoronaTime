// Made by Max Broijl
// Editor tool to make screenshots in editor
// Resolution comes from editor resolution

using UnityEngine;
using UnityEditor;

public class ScreenShotScript : MonoBehaviour {

    public void MakeScreenShot() {
        print(Application.dataPath);
        ScreenCapture.CaptureScreenshot("Assets/ScreenShotTest" + Random.Range(100000000, 999999999) + ".png");
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(ScreenShotScript))]
public class ScreenShotButton : Editor {
    ScreenShotScript target_ScreenShot;
    public void OnEnable() {
        target_ScreenShot = (ScreenShotScript)target;
    }
    public override void OnInspectorGUI() {
        DrawDefaultInspector();
        if (GUILayout.Button("Make screenshot")) {
            target_ScreenShot.MakeScreenShot();
            Debug.LogWarning("Successfully made a screenshot!");
        }
    }

}
#endif