using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowPU : PowerUp {

    [HideInInspector] public Collider thisCollider;
    [HideInInspector] public Rigidbody rigid;
    public float throwForce, angleUp;
    [HideInInspector] public bool inAir;
    public Vector3 extends;
    public Collider[] collidersHit;
    private void Awake() {
        rigid = GetComponent<Rigidbody>();
        rigid.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
        rigid.isKinematic = true;
        rigid.velocity = Vector3.zero;
        thisCollider = GetComponent<Collider>();
    }

    public override void Interact(CartStorage cartStorage) {
        base.Interact(cartStorage);
        thisCollider.isTrigger = true;
    }

    public override void Use() {
        if (!inAir) {
            rigid.isKinematic = false;
            affectedController.useableProduct = null;
            transform.SetParent(null);
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
            collidersHit = Physics.OverlapBox(transform.position, extends, transform.rotation);
            if(collidersHit.Length > 0) {
                for (int i = 0; i < collidersHit.Length; i++) {
                    if (!CheckControllerColliders(collidersHit[i])) {
                        if (collidersHit[i] != thisCollider) {
                            Destroy(rigid);
                            Hit();
                            SetPositionAndRotationToHit();
                        }
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
        print("Hit");
    }

    public void SetPositionAndRotationToHit() {
        float closestDistance = 1000;
        for (int i = 0; i < collidersHit.Length; i++) {
            if (collidersHit[i]) {
                RaycastHit hit;
                Vector3 direction = -(transform.position - collidersHit[i].transform.position).normalized;
                if (Physics.Raycast(transform.position, direction, out hit, Mathf.Infinity)) {
                    float distance = Vector3.Distance(transform.position, hit.point);
                    if (distance < closestDistance) {
                        closestDistance = distance;
                        closestIndex = i;
                        closestHit = hit;
                        //closestRotation = Quaternion.FromToRotation(Vector3.up, closestHit.normal);
                    }
                }
            }
        }
        print(closestHit.transform.gameObject.name);
        transform.position = closestHit.point;
        transform.rotation = Quaternion.FromToRotation(Vector3.up, closestHit.normal);
    }
    int closestIndex = -1;
    RaycastHit closestHit;
    Vector3 closestPoint;
    Quaternion closestRotation;
}