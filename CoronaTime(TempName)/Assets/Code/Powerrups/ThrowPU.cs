﻿using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class ThrowPU : PowerUp {

    [HideInInspector] public Collider thisCollider;
    public float throwForce, angleUp;
    public bool throwChecks, masterThrown;
    public Vector3 extends;
    public Collider[] collidersHit;
    public int closestIndex = -1;
    public LayerMask onlyCollisionOverlapWith = 512;
    private void Awake() {
        rigid = GetComponent<Rigidbody>();
        rigid.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
        rigid.isKinematic = true;
        rigid.velocity = Vector3.zero;
        thisCollider = GetComponent<Collider>();
    }

    public override void Interact(CartStorage cartStorage) {
        base.Interact(cartStorage);
        if (currentPlace == Place.InShelve) {
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
        throwChecks = true;
        masterThrown = true;
    }

    private void Update() {
        if (throwChecks && masterThrown) {
            collidersHit = Physics.OverlapBox(transform.position, extends, transform.rotation, onlyCollisionOverlapWith);
            if (collidersHit.Length > 0) {
                Collision();
            }
        }
    }

    public virtual void Collision() {
        throwChecks = false;
        print("Collision");
        SetPositionAndRotationToHit();
    }

    public void SetPositionAndRotationToHit() {
        RaycastHit hit, closestHit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, 1f)) {
            closestHit = hit;//for possible change
            if (closestHit.transform.GetComponent<Controller>()) {
                Debug.Log("HIT CONTROLLER");
                transform.position = closestHit.point;
                transform.rotation = Quaternion.FromToRotation(Vector3.up, closestHit.normal);
                affectedController = closestHit.transform.GetComponent<Controller>();
                ProductInteractions.pi_Single.SetParentToPhotonView(index, affectedController.photonView.ViewID, RpcTarget.All);
                Vector3 pos = transform.localPosition;
                Vector3 force = Vector3.zero;
                Quaternion rot = transform.localRotation;
                ProductInteractions.pi_Single.SetLocalUseableProductPositionAndRotationAddForceAndSetKinematic(index, pos, force, 1, rot, RpcTarget.All);
                affectedController.powerups_AffectingMe.Add(this);
                if(closestHit.collider.GetType() == typeof(CapsuleCollider)) {
                    gameObject.layer = Manager.staticInformation.int_DontShowTheseLayersLocal;
                }
                affectedCartStorage = affectedController.cartStorage;
                rigid.isKinematic = true;
            }
        }
    }

    private void OnDrawGizmos() {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, transform.forward);
    }
}