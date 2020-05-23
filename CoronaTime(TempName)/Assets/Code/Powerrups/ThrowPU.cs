using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class ThrowPU : PowerUp {

    public Rigidbody rigid;
    public Collider coll;
    public float angleUp;

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
        rigid.isKinematic = false;
        affectedController.useableProduct = null;
        transform.SetParent(null);
        transform.position = affectedController.transform_ThrowFromPoint.position;
        transform.rotation = affectedController.transform_ThrowFromPoint.rotation;
        rigid.AddForce(affectedController.transform_Pov.forward * newValueDuringFX);
        affectedController = null;
        affectedCartStorage = null;
        currentPlace = Place.None;
    }
}
