using UnityEngine;

public class Pausescript : MonoBehaviour
{
    public GameObject pauseMenu, optionsMenu;
    Controller controller;

    private void Start()
    {
        controller = GetComponentInParent<Controller>();
    }
    void Update()
    {
        if (Input.GetButtonDown("Cancel"))
        {
            PauseOrUnpause();
        }
    }

    public void PauseOrUnpause()
    {
        bool currentState = pauseMenu.activeSelf;
        if (currentState)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            pauseMenu.SetActive(false);
        }
        else
        {
            if (optionsMenu.activeSelf) {
                optionsMenu.SetActive(false);
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                currentState = !currentState;
            } else {
                pauseMenu.SetActive(true);
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
        }
        controller.canMove = currentState;
    }
}
