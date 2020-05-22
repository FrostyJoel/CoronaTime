using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable : MonoBehaviourPun {

    public AudioManager.AudioGroups audioGroup = AudioManager.AudioGroups.SFX;

    public enum Place {
        InShelve,
        InCart,
        None
    }

    public Place currentPlace;

    private void OnEnable() {
        transform.tag = "Interact";
    }

    public virtual void Interact(CartStorage cartStorage) {

    }
}
