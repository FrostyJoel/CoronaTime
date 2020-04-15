using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine;

public class SceneSwitchForPresentation : MonoBehaviour {
    public string sceneName;
    public InteractableButton button;

    private void Start() {
        button.Use = SwitchScene;
    }

    public void SwitchScene() {
        SceneManager.LoadScene(sceneName);
    }
}
