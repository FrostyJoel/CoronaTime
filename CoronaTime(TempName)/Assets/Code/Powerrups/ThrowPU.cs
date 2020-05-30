using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class ThrowPU : PowerUp {

    [HideInInspector] public Collider thisCollider;
    public float throwForce, angleUp;
    public bool inAir, masterThrown;
    public Vector3 extends;
    public Collider[] collidersHit;
    public int closestIndex = -1;
    public RaycastHit closestHit;

    private void Awake() {
        rigid = GetComponent<Rigidbody>();
        rigid.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
        rigid.isKinematic = true;
        rigid.velocity = Vector3.zero;
        thisCollider = GetComponent<Collider>();
    }

    public override void Interact(CartStorage cartStorage) {
        base.Interact(cartStorage);
        if(currentPlace == Place.InShelve) {
            ProductInteractions.pi_Single.EnableDisableAllProductColliders(index, false, RpcTarget.All);
        }
    }

    public override void Use() {
        ProductInteractions.pi_Single.ChangePowerUpPlace(index, (int)Place.None, RpcTarget.All);
        affectedController.useableProduct = null;
        ProductInteractions.pi_Single.SetParentToPhotonView(index, -1, RpcTarget.All);
        Vector3 pos = affectedController.transform_ThrowFromPoint.position;
        Quaternion rot = affectedController.transform_ThrowFromPoint.rotation;
        Vector3 force = affectedController.transform_Pov.forward * throwForce;
        ProductInteractions.pi_Single.SetGlobalUseableProductPositionAndRotationAddForceAndSetKinematic(index, pos, force, 0, rot, RpcTarget.All);
        inAir = true;
        masterThrown = true;
    }
    
    private void Update() {
        if (inAir && masterThrown) {
            collidersHit = Physics.OverlapBox(transform.position, extends, transform.rotation);
            if(collidersHit.Length > 0) {
                for (int i = 0; i < collidersHit.Length; i++) {
                    if (!CheckControllerColliders(collidersHit[i])) {
                        Hit();
                    } else {
                        collidersHit[i] = null;
                    }
                }
            }
        }
    }

    bool CheckControllerColliders(Collider collider) {
        bool collision = false;
        if (affectedController) {
            for (int i = 0; i < affectedController.colliders.Length; i++) {
                if (affectedController.colliders[i] == collider) {
                    collision = true;
                    break;
                }
            }
        }
        return collision;
    }

    public virtual void Hit() {
        inAir = false;
        SetPositionAndRotationToHit();
    }

    public void SetPositionAndRotationToHit() {
        float closestDistance = 1000;
        for (int i = 0; i < collidersHit.Length; i++) {
            if (collidersHit[i]) {
                RaycastHit hit;
                if (Physics.Raycast(transform.position, transform.forward, out hit, 1)) {
                    float distance = Vector3.Distance(transform.position, hit.point);
                    if (distance < closestDistance) {
                        closestDistance = distance;
                        closestIndex = i;
                        closestHit = hit;
                    }
                }
            }
        }
        if (closestIndex >= 0) { 
            if (closestHit.transform.GetComponent<Controller>()) {
                affectedController = closestHit.transform.GetComponent<Controller>();
                ProductInteractions.pi_Single.SetParentToPhotonView(index, affectedController.photonView.ViewID, RpcTarget.All);
                Vector3 pos = transform.localPosition;
                Vector3 force = Vector3.zero;
                Quaternion rot = transform.localRotation;
                ProductInteractions.pi_Single.SetLocalUseableProductPositionAndRotationAddForceAndSetKinematic(index, pos, force, 1, rot, RpcTarget.All);
                affectedController.powerups_AffectingMe.Add(this);
                affectedCartStorage = affectedController.cartStorage;
                rigid.isKinematic = true;
            } else {
                affectedCartStorage = null;
                affectedController = null;
                ProductInteractions.pi_Single.EnableDisableAllProductColliders(index, true, RpcTarget.All);
            }
        }
    }
}