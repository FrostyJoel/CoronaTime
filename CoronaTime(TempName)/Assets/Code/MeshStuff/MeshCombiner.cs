﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// Copy meshes from children into the parent's Mesh.
// CombineInstance stores the list of meshes.  These are combined
// and assigned to the attached Mesh.

public class MeshCombiner : MonoBehaviour
{
    void Start()
    {
        ActualMeshCombining();
    }

    public void ActualMeshCombining()
    {
        List<MeshFilter> meshFilters = new List<MeshFilter>();
        MeshRenderer mR = gameObject.AddComponent<MeshRenderer>();
        List<Material> matL = new List<Material>();
        int i = 0;
        for (int j = 0; j < transform.childCount; j++)
        {
            if (transform.GetChild(j).GetComponent<MeshFilter>())
            {
                Transform childJ = transform.GetChild(j);
                meshFilters.Add(childJ.GetComponent<MeshFilter>());
                matL.Add(childJ.GetComponent<MeshRenderer>().material);
            }
        }
        CombineInstance[] combine = new CombineInstance[meshFilters.Count];
        mR.materials = matL.ToArray();
        while (i < meshFilters.Count)
        {
            combine[i].mesh = meshFilters[i].sharedMesh;
            combine[i].transform = meshFilters[i].transform.localToWorldMatrix;
            DestroyImmediate(meshFilters[i]);
            i++;
        }
        Mesh mesh = new Mesh();
        mesh.CombineMeshes(combine,false,true,false);
        MeshFilter mF = gameObject.AddComponent<MeshFilter>();
        mF.mesh = mesh;
        transform.gameObject.SetActive(true);
        print(gameObject.name + " Vertices: " + mF.mesh.vertices.Length);
    }
}