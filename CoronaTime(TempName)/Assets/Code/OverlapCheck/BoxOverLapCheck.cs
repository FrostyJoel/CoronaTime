using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxOverLapCheck : MonoBehaviour {

    BoxCollider col;
    public Collider[] allcoll;
    public Collider colll;
    public LayerMask mask;

    private void Reset() {
        mask = ~(1 << 0);
    }

    private void Start() {
        col = GetComponent<BoxCollider>();
        if (col) {
            Collider[] cols = Physics.OverlapBox(transform.position + col.center, col.bounds.extents, transform.rotation, mask);
            if(cols.Length > 0) {
                for (int i = 0; i < cols.Length; i++) {
                    if (cols[i] != col) {
                        colll = cols[i];
                        Lap();
                        allcoll = cols;
                    }
                }
            }
        }
    }

    void Lap() {
        MeshRenderer mesh = GetComponent<MeshRenderer>();
        if (mesh) {
            mesh.material.color = Color.red;
        }
    }
}