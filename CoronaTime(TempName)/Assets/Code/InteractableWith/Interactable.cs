using Photon.Pun;
using UnityEngine;

public class Interactable : MonoBehaviourPun {

    public AudioClip clip;
    public AudioManager.AudioGroups audioGroup = AudioManager.AudioGroups.SFX;

    public bool interactable;
    [Space]
    public SpecialPosAndRot SpecialPosAndRot;

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

    public void PlaySound() {
        if (clip) {
            AudioManager.PlaySound(clip, audioGroup);
        }
    }
}

[System.Serializable]
public class SpecialPosAndRot {
    public bool use;
    public Vector3 pos, rot;
}