using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class BulkPhysicMatPlacer : MonoBehaviour {
    
    [Tooltip("when variable is empty it won't be used")]
    public bool execute;
    public PhysicMaterial mat;

    void Update() {
        if (execute) {
            if (mat) {
                Collider[] colliders = GetComponentsInChildren<Collider>();
                if(colliders.Length > 0) {
                    for (int i = 0; i < colliders.Length; i++) {
                        colliders[i].material = mat;
                    }
                }
            }
            DestroyImmediate(this);
        }
    }
}
