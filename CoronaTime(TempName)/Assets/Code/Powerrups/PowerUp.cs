using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class PowerUp : Interactable {
    public float newValueDuringFX, durationInSeconds;
    [Header("Particle")]
    public VisualFX particleToUse;

    [Header("FX Sound")]
    public AudioClip fxClip;
    
    public enum FX {
        Speed,
        Stun,
        Glow,
        Block
    }

    public FX thisFx;

    [Header("Images")]
    public Sprite puImage;

    [HideInInspector] public int index;
    [HideInInspector] public Vector3 hitPos;
    [HideInInspector] public Rigidbody rigid;
    [HideInInspector] public bool inUse, setAsPU;
    [HideInInspector] public float durationSpentInSeconds;
    [HideInInspector] public Controller affectedController;
    [HideInInspector] public CartStorage affectedCartStorage;

    public virtual void Use() {
        if (affectedController) {
            affectedController.SetAffectingFX(this);
            affectedController.useableProduct = null;
            affectedCartStorage.heldPUImageHolder.sprite = null;
            affectedCartStorage.heldPUImageHolder.color = Vector4.zero;
        }
    }

    public virtual void UseEffect() {
        if(durationInSeconds >= 0) {
            if (durationSpentInSeconds < durationInSeconds) {
                Effect();
                durationSpentInSeconds += Time.deltaTime;
            } else {
                StopUsing();
            }
        } else {
            Effect();
        }
    }    

    public virtual void Effect() {

    }

    public virtual void StartStopParticle(bool start) {
        if (particleToUse) {
            for (int i = 0; i < affectedController.particles.Length; i++) {
                if(affectedController.particles[i].name == particleToUse.name) {
                    ProductInteractions.pi_Single.StartStopParticleOnPlayer(i, affectedController.photonView.ViewID, hitPos, start, true, RpcTarget.All);
                    break;
                }
            }
        }
    }

    public override void Interact(CartStorage cartStorage) {
        Vector3 interactPos = transform.position;
        if (currentPlace == Place.InShelve && cartStorage.SetPowerUp(index)) {
            ProductInteractions.pi_Single.PlaypickUpSoundAndInstantiateParticleOnUseableProduct(index, interactParticleDestroyTime, true, false, interactPos, RpcTarget.All);
            ProductInteractions.pi_Single.ChangePowerUpPlace(index, (int)Place.InCart, RpcTarget.All);
            affectedCartStorage = cartStorage;
            affectedController = cartStorage.controller;
            if (GetComponent<Outline>()) {
                GetComponent<Outline>().enabled = false;
            }
        }
    }

    public void PlayFXSound() {
        if (fxClip) {
            AudioManager.PlaySound(fxClip, audioGroup, transform.position);
        }
    }

    public virtual void StopUsing() {
        affectedController.powerups_AffectingMe.Remove(this);
        for (int i = 0; i < affectedCartStorage.visualPuFXList.Count; i++) {
            if(affectedCartStorage.visualPuFXList[i].thisFX == thisFx) {
                Color defCol = affectedCartStorage.visualPuFXList[i].fxImage.color;
                defCol.a = affectedCartStorage.visualPuFXList[i].defaultTransparency;
                affectedCartStorage.visualPuFXList[i].fxImage.color = defCol;
            }
        }
        StartStopParticle(false);
        ProductInteractions.pi_Single.DestroyUseAbleProduct(index, 0, RpcTarget.All);
    }
}