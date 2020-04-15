using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableButton : Interactable {

    public delegate void use();
    public use Use;

    public override void Interact(CartStorage cartStorage) {
        print("Use");
        Use();
    }
}
