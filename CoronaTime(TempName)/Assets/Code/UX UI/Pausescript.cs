using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pausescript : MonoBehaviour
{
    public GameObject pauseMenu;
    public Controller p;

    private void Start()
    {
        p = GetComponentInParent<Controller>();
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
            Cursor.visible = true;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = false;
        }
        pauseMenu.SetActive(!currentState);
        p.canMove = currentState;
    }
}
