using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowPU : PowerUp {

    public Collider coll;
    public Rigidbody rigid;
    public float throwForce, angleUp;
    public bool inAir;
    public Vector3 extends;
    private void Awake() {
        rigid = GetComponent<Rigidbody>();
        rigid.isKinematic = true;
        rigid.velocity = Vector3.zero;
        coll = GetComponent<Collider>();
    }

    public override void Interact(CartStorage cartStorage) {
        base.Interact(cartStorage);
        coll.isTrigger = true;
    }

    public override void Use() {
        if (!inAir) {
            print("Use");
            rigid.isKinematic = false;
            affectedController.useableProduct = null;
            transform.SetParent(null);
            transform.position = affectedController.transform_ThrowFromPoint.position;
            Vector3 rot = affectedController.transform_ThrowFromPoint.rotation.eulerAngles;
            rot.x += angleUp;
            transform.rotation = Quaternion.Euler(rot);
            rigid.AddForce(affectedController.transform_Pov.forward * throwForce);
            //affectedController = null;
            //affectedCartStorage = null;
            currentPlace = Place.None;
            inAir = true;
        }
    }

    private void Update() {
        if (inAir) {
            Collider[] colliders = Physics.OverlapBox(transform.position, extends, transform.rotation);
            if(colliders.Length > 0) {
                for (int i = 0; i < colliders.Length; i++) {
                    if (!CheckControllerColliders(colliders[i]) && colliders[i] != coll) {
                        print(colliders[i].gameObject.name);
                        Hit();
                        Destroy(rigid);
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
    }

    //private void OnTriggerEnter(Collider other) {
    //    if (currentPlace == Place.None && !CheckControllerColliders(other)) {
    //        Destroy(rigid);
    //        print("collision" + gameObject.name);   
    //    }
    //}

    private void OnDrawGizmos() {
       // Gizmos.
    }

}