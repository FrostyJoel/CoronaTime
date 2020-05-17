using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pausescript : MonoBehaviour
{
    public GameObject pauseMenu;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButton("Cancel"))
        {
            pauseMenu.SetActive(true);
        }
    }
}
