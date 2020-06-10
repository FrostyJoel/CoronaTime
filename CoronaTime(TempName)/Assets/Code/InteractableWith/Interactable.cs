using Photon.Pun;
using UnityEngine;

public class Interactable : MonoBehaviourPun {

    public AudioClip clip;
    public AudioManager.AudioGroups audioGroup = AudioManager.AudioGroups.SFX;

    public bool interactable;
    [Space]
    public ProductManipulation SpecialPosAndRot;

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
            AudioManager.PlaySound(clip, transform.position, audioGroup);
        }
    }
}

[System.Serializable]
public class ProductManipulation {
    public bool use;
    public Vector3 pos, rot;
    public ProductScaling productScaling;
}

[System.Serializable]
public class ProductScaling {
    public bool useNewScale;
    public Vector3 scale;
}