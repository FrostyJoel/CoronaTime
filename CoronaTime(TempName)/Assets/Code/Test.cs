using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour {
    public GameObject gamemanagerPrefab;

    private void Start() {
        Instantiate(gamemanagerPrefab);
    }
}
