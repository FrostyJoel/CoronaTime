using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zone : MonoBehaviour {
    public int productsToFind;
    [Header("HideInInspector")]
    public List<Interactable> productsInZone = new List<Interactable>();

    private void Awake() {
        Interactable[] temp = GetComponentsInChildren<Interactable>();
    }
}
