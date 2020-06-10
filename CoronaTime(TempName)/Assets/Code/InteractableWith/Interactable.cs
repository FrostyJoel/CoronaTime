using Photon.Pun;
using UnityEngine;

public class Interactable : MonoBehaviourPun {

    public AudioClip interactClip;
    public AudioManager.AudioGroups audioGroup = AudioManager.AudioGroups.SFX;
    public bool interactable;
    [Space]
    public ProductManipulation SpecialPosAndRot;

    [Header("Interact particle")]
    public GameObject interactParticle;
    public float interactParticleDestroyTime;

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

    public void PlayInteractSound() {
        if (interactClip) {
            AudioManager.PlaySound(interactClip, transform.position, audioGroup);
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