using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class InteractableCashRegister : Interactable {

    public Text text;
    public AudioClip sellSound;

    public override void Interact(CartStorage cartStorage) {
        cartStorage.SellItems();
        cartStorage.controller.ResetAtStartPosition();
        if (sellSound) {
            AudioManager.PlaySound(sellSound, audioGroup);
        }
        if (text) {
            text.text = cartStorage.GetScore().ToString();
        }
        print(cartStorage.GetScore());
    }
}