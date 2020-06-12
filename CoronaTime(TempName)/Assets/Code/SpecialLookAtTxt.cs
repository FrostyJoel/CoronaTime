using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecialLookAtTxt : MonoBehaviour {
    public string hoverText;

    private void Reset() {
        gameObject.layer = 18;
    }
}
