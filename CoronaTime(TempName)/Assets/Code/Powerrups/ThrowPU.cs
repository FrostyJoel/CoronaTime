﻿using Photon.Pun;
using UnityEngine;

public class ThrowPU : PowerUp {

    public float throwForce = 100;
    public Vector3 extends, gizmosCenter;
    public LayerMask onlyCollisionOverlapWith = 512;
    public Transform newCollisionRaycastPositionIfNeeded;
    [Space]
    public bool showGizmos;
    [HideInInspector] public Collider[] collidersHit;
    [HideInInspector] public Collider[] theseColliders;
    [HideInInspector] public bool throwChecks, masterThrown;

    private void Awake() {
        rigid = GetComponent<Rigidbody>();
        rigid.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
        rigid.isKinematic = true;
        rigid.velocity = Vector3.zero;
        theseColliders = GetComponents<Collider>();
    }

    public override void Interact(CartStorage cartStorage) {
        base.Interact(cartStorage);
        if (currentPlace != Place.InShelve) {
            ProductInteractions.pi_Single.EnableDisableAllProductColliders(index, false, RpcTarget.All);
        }
    }

    public override void Use() {
        ProductInteractions.pi_Single.ChangePowerUpPlace(index, (int)Place.None, RpcTarget.All);
        affectedController.useableProduct = null;
        affectedCartStorage.heldPUImageHolder.sprite = null;
        affectedCartStorage.heldPUImageHolder.color = Vector4.zero;
        ProductInteractions.pi_Single.SetParentToPhotonView(index, -1, RpcTarget.All);
        Vector3 pos = affectedController.transform_ThrowFromPoint.position;
        Quaternion rot = affectedController.transform_ThrowFromPoint.rotation;
        Vector3 force = affectedController.transform_Pov.forward * throwForce;
        ProductInteractions.pi_Single.EnableDisableAllProductColliders(index, true, RpcTarget.All);
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
        SetPositionAndRotationToHit();
    }

    public void SetPositionAndRotationToHit() {
        RaycastHit hit;
        Transform raycastPostion = transform;
        if (newCollisionRaycastPositionIfNeeded) {
            raycastPostion = newCollisionRaycastPositionIfNeeded;
        }
        LayerMask mask = ~(1 << 11);
        if (Physics.Raycast(raycastPostion.position, raycastPostion.forward, out hit, 1f, mask)) {
            Controller hitController = hit.transform.GetComponent<Controller>();
            print(hit.transform.gameObject.name);
            if (hitController) {
                transform.position = hit.point;
                transform.rotation = Quaternion.FromToRotation(Vector3.up, hit.normal);
                ProductInteractions.pi_Single.SetParentToPhotonView(index, hitController.photonView.ViewID, RpcTarget.All);
                hitPos = transform.localPosition;
                Vector3 pos = transform.localPosition;
                Vector3 force = Vector3.zero;
                Quaternion rot = transform.localRotation;
                ProductInteractions.pi_Single.SetLocalUseableProductPositionAndRotationAddForceAndSetKinematic(index, pos, force, 1, rot, RpcTarget.All);
                bool capsuleHit = false;
                if (hit.collider.GetType() == typeof(CapsuleCollider)) {
                    capsuleHit = true;
                }
                ProductInteractions.pi_Single.SetAffectedController(index, hitController.photonView.ViewID, capsuleHit, RpcTarget.All);
            }
        }
    }

    private void OnDrawGizmos() {
        if(newCollisionRaycastPositionIfNeeded && showGizmos) {
            Debug.DrawRay(newCollisionRaycastPositionIfNeeded.position, newCollisionRaycastPositionIfNeeded.forward);
        }
        Vector3 posToUse = transform.position;
        if(gizmosCenter != Vector3.zero) {
            posToUse = transform.position + gizmosCenter;
        }
        Gizmos.matrix = Matrix4x4.TRS(posToUse, transform.localRotation, extends);
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(Vector3.zero, Vector3.one);
    }
}