using UnityEngine;
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
        for (int i = 0; i < meshes.Count; i++) {
            InstanceFilterInstance(meshes[i]);
        }
    }

    void InstanceFilterInstance(Meshes ms) {
        CombineInstance[] combine = new CombineInstance[ms.meshList.Count];

        Mesh combinedMesh = new Mesh();
        combinedMesh.name = "Material:" + ms.meshList[0].name;
        combinedMesh.CombineMeshes(combine, true, true, false);

        GameObject g = new GameObject(ms.meshList[0].name);

        MeshFilter filter = g.AddComponent<MeshFilter>();
        filter.mesh = combinedMesh;

        MeshRenderer render = g.AddComponent<MeshRenderer>();
        render.material = ms.material;
    }

    void CheckForMesh(Transform t) {
        if(t.GetComponent<MeshFilter>() && t.GetComponent<MeshRenderer>()) {
            MeshFilter filter = t.GetComponent<MeshFilter>();
            int index = MeshWhereInMeshes(filter.mesh);

            if (index >= 0) {
                meshes[index].meshList.Add(filter.mesh);
            } else {
                Meshes mesh = new Meshes();
                mesh.meshList.Add(filter.mesh);
                mesh.material = t.GetComponent<MeshRenderer>().material;
                meshes.Add(mesh);
            }
        }

        if (t.childCount > 0) {
            for (int i = 0; i < t.childCount; i++) {
                CheckForMesh(t.GetChild(i));
            }
        }
    }
    
    int MeshWhereInMeshes(Mesh mesh) {
        int i = -1;
        if (meshes.Count > 0) {
            for (int iB = 0; iB < meshes.Count; iB++) {
                if (meshes[iB].meshList.Count > 0) {
                    for (int iC = 0; iC < meshes[iB].meshList.Count; iC++) {
                        if (mesh.name == meshes[iB].meshList[iC].name) {
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
    public List<Mesh> meshList = new List<Mesh>();
    public Material material;
}