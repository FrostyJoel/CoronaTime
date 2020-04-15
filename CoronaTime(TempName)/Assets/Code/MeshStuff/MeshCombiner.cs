﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// Copy meshes from children into the parent's Mesh.
// CombineInstance stores the list of meshes.  These are combined
// and assigned to the attached Mesh.

[ExecuteInEditMode]
public class MeshCombiner : MonoBehaviour
{

    public bool execute;

    public List<Meshes> meshes = new List<Meshes>();

    void Update() {
        if (execute) {
            CombineMesh();
            execute = false;
        }
    }

    void CombineMesh() {
        CheckForMesh(transform);
        for (int i = 0; i < meshAndMatList.Count; i++) {
            InstanceFilterInstance(meshAndMatList[i]);
        }
    }

    void InstanceFilterInstance(Meshes ms) {
        CombineInstance[] combine = new CombineInstance[ms.meshFilterList.Count];

        for (int i = 0; i < ms.meshFilterList.Count; i++) {
            combine[i].mesh = ms.meshFilterList[i].sharedMesh;
            combine[i].transform = ms.meshFilterList[i].transform.localToWorldMatrix;
            ms.meshFilterList[i].gameObject.SetActive(false);
        }

        Mesh combinedMesh = new Mesh();
        combinedMesh.name = "Material:" + ms.meshFilterList[0].name;
        combinedMesh.CombineMeshes(combine, true, true, false);

        GameObject g = new GameObject(ms.meshFilterList[0].name);

        MeshFilter filter = g.AddComponent<MeshFilter>();
        filter.mesh = combinedMesh;

        MeshRenderer render = g.AddComponent<MeshRenderer>();
        render.material = ms.material;
    }

    void CheckForMesh(Transform t) {
        if(t.GetComponent<MeshFilter>() && t.GetComponent<MeshRenderer>()) {
            MeshFilter filter = t.GetComponent<MeshFilter>();
            int index = MeshWhereInMeshes(filter);

            if (index >= 0) {
                meshAndMatList[index].meshFilterList.Add(filter);
            } else {
                Meshes tempMesh = new Meshes();
                tempMesh.meshFilterList.Add(filter);
                tempMesh.material = t.GetComponent<MeshRenderer>().sharedMaterial;
                meshAndMatList.Add(tempMesh);
            }
        }

        if (t.childCount > 0) {
            for (int i = 0; i < t.childCount; i++) {
                CheckForMesh(t.GetChild(i));
            }
        }
    }
    
    int MeshWhereInMeshes(MeshFilter filter) {
        int i = -1;
        if (meshAndMatList.Count > 0) {
            for (int iB = 0; iB < meshAndMatList.Count; iB++) {
                if (meshAndMatList[iB].meshFilterList.Count > 0) {
                    for (int iC = 0; iC < meshAndMatList[iB].meshFilterList.Count; iC++) {
                        if (filter.sharedMesh.name == meshAndMatList[iB].meshFilterList[iC].sharedMesh.name) {
                            i = iB;
                            break;
                        }
                    }
                }
            }
        }
        return i;
    }
}

[System.Serializable]
public class Meshes {
    public List<MeshFilter> meshFilterList = new List<MeshFilter>();
    public Material material;
}