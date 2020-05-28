using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class ThrowPU : PowerUp {

    [HideInInspector] public Collider thisCollider;
    [HideInInspector] public Rigidbody rigid;
    public float throwForce, angleUp;
    [HideInInspector] public bool inAir;
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
        Destroy(thisCollider);
    }

    public override void Use() {
        if (!inAir) {
            rigid.isKinematic = false;
            affectedController.useableProduct = null;
            ProductInteractions.pi_Single.UnParentProduct(index, RpcTarget.All);
            transform.position = affectedController.transform_ThrowFromPoint.position;
            Vector3 rot = affectedController.transform_ThrowFromPoint.rotation.eulerAngles;
            rot.x += angleUp;
            transform.rotation = Quaternion.Euler(rot);
            rigid.AddForce(affectedController.transform_Pov.forward * throwForce);
            currentPlace = Place.None;
            inAir = true;
        }
    }
    
    private void Update() {
        if (inAir) {
            ProductInteractions.pi_Single.SetProductPosition(index, transform.position, transform.rotation, RpcTarget.All);
            collidersHit = Physics.OverlapBox(transform.position, extends, transform.rotation);
            if(collidersHit.Length > 0) {
                for (int i = 0; i < collidersHit.Length; i++) {
                    if (!CheckControllerColliders(collidersHit[i])) {
                        rigid.isKinematic = true;
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
        for (int i = 0; i < affectedController.colliders.Length; i++) {
            if (affectedController.colliders[i] == collider) {
                collision = true;
                break;
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
            ProductInteractions.pi_Single.SetProductPosition(index, closestHit.point, Quaternion.FromToRotation(Vector3.up, closestHit.normal), RpcTarget.All);
            if (closestHit.transform.GetComponent<Controller>()) {
                affectedController = closestHit.transform.GetComponent<Controller>();
                affectedCartStorage = affectedController.cartStorage;
            } else {
                affectedCartStorage = null;
                affectedController = null;
            }
        }
    }
}