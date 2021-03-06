﻿using Photon.Pun;
using UnityEngine;

public class ProductInteractions : MonoBehaviourPun {
    public static ProductInteractions pi_Single;

    private void Awake() {
        pi_Single = this;
    }

    public void DestroyProduct(int index, float time, RpcTarget selectedTarget) {
        photonView.RPC("RPC_DestroyProduct", selectedTarget, index, time);
    }

    public void DestroyUseAbleProduct(int index, float time, RpcTarget selectedTarget) {
        photonView.RPC("RPC_DestroyUseAbleProduct", selectedTarget, index, time);
    }

    public void DestroyAllProductColliders(int index, RpcTarget selectedTarget) {
        photonView.RPC("RPC_DestroyAllProductColliders", selectedTarget, index);
    }

    public void EnableDisableAllProductColliders(int index, bool state, RpcTarget selectedTarget) {
        photonView.RPC("RPC_EnableDisableAllProductColliders", selectedTarget, index, state);
    }

    public void AddToCart(int index, int id, RpcTarget selectedTarget) {
        photonView.RPC("RPC_AddToCart", selectedTarget, index, id);
    }

    public void SetPowerUp(int index, int id, RpcTarget selectedTarget) {
        photonView.RPC("RPC_SetPowerUp", selectedTarget, index, id);
    }

    public void SetGlobalUseableProductPositionAndRotation(int index, Vector3 pos, Quaternion rot, RpcTarget selectedTarget) {
        photonView.RPC("RPC_SetGlobalUseableProductPositionAndRotation", selectedTarget, index, pos, rot);
    }

    public void SetLocalInteractableProductPositionAndRotation(int index, Vector3 pos, Quaternion rot, RpcTarget selectedTarget) {
        photonView.RPC("RPC_SetLocalInteractableProductPositionAndRotation", selectedTarget, index, pos, rot);
    }

    public void SetLocalInteractableProductScale(int index, Vector3 scale, RpcTarget selectedTarget) {
        photonView.RPC("RPC_SetLocalInteractableProductScale", selectedTarget, index, scale);
    }

    public void SetGlobalUseableProductPositionAndRotationAddForceAndSetKinematic(int index, Vector3 pos, Vector3 force, int kinematicState, Quaternion rot, RpcTarget selectedTarget) {
        photonView.RPC("RPC_SetGlobalUseableProductPositionAndRotationAddForceAndSetKinematic", selectedTarget, index, pos, force, kinematicState, rot);
    }

    public void SetLocalUseableProductPositionAndRotationAddForceAndSetKinematic(int index, Vector3 pos, Vector3 force, int kinematicState, Quaternion rot, RpcTarget selectedTarget) {
        photonView.RPC("RPC_SetLocalUseableProductPositionAndRotationAddForceAndSetKinematic", selectedTarget, index, pos, force, kinematicState, rot);
    }

    public void SetLocalUseableProductPositionAndRotation(int index, Vector3 pos, Quaternion rot, RpcTarget selectedTarget) {
        photonView.RPC("RPC_SetLocalUseableProductPositionAndRotation", selectedTarget, index, pos, rot);
    }

    public void SetParentToPhotonView(int index, int id, RpcTarget selectedTarget) {
        photonView.RPC("RPC_SetParentToPhotonView", selectedTarget, index, id);
    }

    public void DisableVisibility(int index, int id, bool local, RpcTarget selectedTarget) {
        int isLocal = 0;
        if (local) {
            isLocal = 1;
        }
        photonView.RPC("RPC_DisableVisibility", selectedTarget, index, id, isLocal);
    }

    public void ChangeProductPlace(int index, int placeIndex, RpcTarget selectedTarget) {
        photonView.RPC("RPC_ChangeProductPlace", selectedTarget, index, placeIndex);
    }

    public void ChangePowerUpPlace(int index, int placeIndex, RpcTarget selectedTarget) {
        photonView.RPC("RPC_ChangePowerUpPlace", selectedTarget, index, placeIndex);
    }

    public void SetAffectedController(int index, int id, bool capsuleHit, RpcTarget selectedTarget) {
        int wasCapsuleHit = 0;
        if (capsuleHit) {
            wasCapsuleHit = 1;
        }
        photonView.RPC("RPC_SetAffectedController", selectedTarget, index, id, wasCapsuleHit);
    }

    public void EnableDisableControllerOutline(int id, bool enableOutline, RpcTarget selectedTarget) {
        int enable = 0;
        if (enableOutline) {
            enable = 1;
        }
        photonView.RPC("RPC_EnableDisableControllerOutline", selectedTarget, id, enable);
    }

    public void StartStopParticleOnPlayer(int index, int id, Vector3 pos, bool play, bool shouldBeLocal, RpcTarget selectedTarget) {
        int startPlaying = 0;
        if (play) {
            startPlaying = 1;
        }
        int local = 0;
        if (shouldBeLocal) {
            local = 1;
        }
        photonView.RPC("RPC_StartStopParticleOnPlayer", selectedTarget, index, id, pos, startPlaying, local);
    }

    public void PlaypickUpSoundAndInstantiateParticleOnUseableProduct(int index, float destroyTime, bool playSound, bool isInteractOrFx, Vector3 pos, RpcTarget selectedTarget) {
        int sound = 0;
        if (playSound) {
            sound = 1;
        }
        int interactOrFX = 0;
        if (isInteractOrFx) {
            interactOrFX = 1;
        }
        photonView.RPC("RPC_PlaypickUpSoundAndInstantiateParticleOnUseableProduct", selectedTarget, index, sound, interactOrFX, destroyTime, pos);
    }

    public void PlaypickUpSoundAndInstantiateParticleOnInteractableProduct(int index, float destroyTime, bool playSound, Vector3 pos, RpcTarget selectedTarget) {
        int sound = 0;
        if (playSound) {
            sound = 1;
        }
        photonView.RPC("RPC_PlaypickUpSoundAndInstantiateParticleOnInteractableProduct", selectedTarget, index, sound, destroyTime, pos);
    }

    [PunRPC]
    void RPC_DestroyProduct(int index, float time) {
        try {
            Destroy(PhotonProductList.staticInteratableProductList[index].gameObject, time);
        } catch { }
    }

    [PunRPC]
    void RPC_DestroyUseAbleProduct(int index, float time) {
        try {
            if (!PhotonProductList.staticUseableProductList[index].GetComponent<PowerUp>().setAsPU) {
                Destroy(PhotonProductList.staticUseableProductList[index].gameObject, time);
            }
        } catch { }
    }

    [PunRPC]
    void RPC_DestroyAllProductColliders(int index) {
        try {
            Collider[] colliders = PhotonProductList.staticUseableProductList[index].gameObject.GetComponentsInChildren<Collider>();
            for (int i = colliders.Length - 1; i >= 0; i--) {
                Destroy(colliders[i]);
            }
        } catch { }
    }

    [PunRPC]
    void RPC_EnableDisableAllProductColliders(int index, bool state) {
        try {
            Collider[] colliders = PhotonProductList.staticUseableProductList[index].gameObject.GetComponentsInChildren<Collider>();
            for (int i = colliders.Length - 1; i >= 0; i--) {
                colliders[i].enabled = state;
            }
        } catch { }
    }

    [PunRPC]
    void RPC_AddToCart(int index, int id) {
        CartStorage storage = PhotonNetwork.GetPhotonView(id).GetComponent<CartStorage>();
        GameObject productObject = PhotonProductList.staticInteratableProductList[index].gameObject;
        storage.heldProducts.Add(PhotonProductList.staticInteratableProductList[index].scriptableProduct);
        storage.heldProductModels.Add(productObject);
        productObject.transform.SetParent(storage.itemHolders[storage.heldProducts.Count - 1]);
        productObject.transform.localPosition = Vector3.zero;
        productObject.transform.localRotation = Quaternion.Euler(Vector3.zero);
    }

    [PunRPC]
    void RPC_SetPowerUp(int index, int id) {
        CartStorage storage = PhotonNetwork.GetPhotonView(id).GetComponent<CartStorage>();
        GameObject productObject = PhotonProductList.staticUseableProductList[index].gameObject;
        productObject.transform.SetParent(storage.transform_PowerUpHolder);
        productObject.transform.localPosition = Vector3.zero;
        productObject.transform.localRotation = Quaternion.Euler(Vector3.zero);
    }

    [PunRPC]
    void RPC_SetLocalInteractableProductPositionAndRotation(int index, Vector3 pos, Quaternion rot) {
        PhotonProductList.staticInteratableProductList[index].transform.localPosition = pos;
        PhotonProductList.staticInteratableProductList[index].transform.localRotation = rot;
    }

    [PunRPC]
    void RPC_SetLocalInteractableProductScale(int index, Vector3 scale) {
        PhotonProductList.staticInteratableProductList[index].transform.localScale = scale;
    }

    [PunRPC]
    void RPC_SetGlobalUseableProductPositionAndRotation(int index, Vector3 pos, Quaternion rot) {
        PhotonProductList.staticUseableProductList[index].transform.position = pos;
        PhotonProductList.staticUseableProductList[index].transform.rotation = rot;
    }

    [PunRPC]
    void RPC_SetLocalUseableProductPositionAndRotation(int index, Vector3 pos, Quaternion rot) {
        PhotonProductList.staticUseableProductList[index].transform.localPosition = pos;
        PhotonProductList.staticUseableProductList[index].transform.localRotation = rot;
    }

    [PunRPC]
    void RPC_SetGlobalUseableProductPositionAndRotationAddForceAndSetKinematic(int index, Vector3 pos, Vector3 force, int kinematicState, Quaternion rot) {
        bool makeKinematic = false;
        if(kinematicState == 1) {
            makeKinematic = true;
        }

        PhotonProductList.staticUseableProductList[index].transform.position = pos;
        PhotonProductList.staticUseableProductList[index].transform.rotation = rot;
        if (PhotonProductList.staticUseableProductList[index].rigid) {
            PhotonProductList.staticUseableProductList[index].rigid.isKinematic = makeKinematic;
        }
        PhotonProductList.staticUseableProductList[index].rigid.AddForce(force);
    }

    [PunRPC]
    void RPC_SetLocalUseableProductPositionAndRotationAddForceAndSetKinematic(int index, Vector3 pos, Vector3 force, int kinematicState, Quaternion rot) {
        bool makeKinematic = false;
        if(kinematicState == 1) {
            makeKinematic = true;
        }

        PhotonProductList.staticUseableProductList[index].transform.localPosition = pos;
        PhotonProductList.staticUseableProductList[index].transform.localRotation = rot;
        PhotonProductList.staticUseableProductList[index].rigid.isKinematic = makeKinematic;
        PhotonProductList.staticUseableProductList[index].rigid.AddForce(force);
    }

    [PunRPC]
    void RPC_SetParentToPhotonView(int index, int id) {
        Transform parentTransform = null;
        if(id >= 0) {
            parentTransform = PhotonNetwork.GetPhotonView(id).transform;
        }
        PhotonProductList.staticUseableProductList[index].transform.SetParent(parentTransform);
    }

    [PunRPC]
    void RPC_DisableVisibility(int index, int id, int local) {
        if (local == 1) {
            if (PhotonNetwork.GetPhotonView(id).Owner.IsLocal) {
                PhotonProductList.staticUseableProductList[index].gameObject.layer = Manager.staticInformation.int_DontShowTheseLayersLocal;
            }
        } else {
            PhotonProductList.staticUseableProductList[index].gameObject.layer = Manager.staticInformation.int_DontShowTheseLayersLocal;
        }
    }

    [PunRPC]
    void RPC_ChangeProductPlace(int index, int placeIndex) {
        PhotonProductList.staticInteratableProductList[index].currentPlace = (Interactable.Place)placeIndex;
    }

    [PunRPC]
    void RPC_ChangePowerUpPlace(int index, int placeIndex) {
        PhotonProductList.staticUseableProductList[index].currentPlace = (Interactable.Place)placeIndex;
    }

    [PunRPC]
    void RPC_SetAffectedController(int index, int id, int capsuleHit) {
        PhotonView pv = PhotonNetwork.GetPhotonView(id);
        if (pv.Owner.IsLocal) {
            PowerUp pu = PhotonProductList.staticUseableProductList[index];
            pu.affectedController = pv.GetComponent<Controller>();
            //pu.affectedController.powerups_AffectingMe.Add(pu);
            pu.affectedController.SetAffectingFX(pu);
            pu.affectedCartStorage = pu.affectedController.cartStorage;
            if (capsuleHit == 1) {
                pu.gameObject.layer = Manager.staticInformation.int_DontShowTheseLayersLocal;
            }
        }
    }

    [PunRPC]
    void RPC_EnableDisableControllerOutline(int id, int enableOutline) {
        bool enable = false;
        if(enableOutline == 1) {
            enable = true;
        }
        PhotonNetwork.GetPhotonView(id).GetComponent<Controller>().myOutline.enabled = enable;
    }

    [PunRPC]
    void RPC_StartStopParticleOnPlayer(int index, int id, Vector3 pos, int play, int shouldBeLocal) {
        bool shouldPlay = false;
        if(play == 1) {
            shouldPlay = true;
        }
        bool local = false;
        if(shouldBeLocal == 1) {
            local = true;
        }
        PhotonNetwork.GetPhotonView(id).GetComponent<Controller>().particles[index].StartStopVisualFX(shouldPlay, pos, local);
    }

    [PunRPC]
    void RPC_PlaypickUpSoundAndInstantiateParticleOnInteractableProduct(int index, int playSound, float destroyTime, Vector3 pos) {
        InteractableProduct ip = PhotonProductList.staticInteratableProductList[index];
        if(playSound == 1) {
            ip.PlayInteractSound();
        }
        if (ip.interactParticle) {
            GameObject ps = Instantiate(ip.interactParticle, pos, Quaternion.identity);
            Destroy(ps, destroyTime);
        }
    }

    [PunRPC]
    void RPC_PlaypickUpSoundAndInstantiateParticleOnUseableProduct(int index, int playSound, int interactOrFX, float destroyTime, Vector3 pos) {
        PowerUp pu = PhotonProductList.staticUseableProductList[index];
        if (playSound == 1) {
            if (interactOrFX == 0) {
                pu.PlayInteractSound();
            } else {
                pu.PlayFXSound();
            }
        }

        if (pu.interactParticle) {
            GameObject ps = Instantiate(pu.interactParticle, pos, Quaternion.identity);
            Destroy(ps, destroyTime);
        }
    }
}