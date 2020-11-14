using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Rendering;
using System.Threading;

[RequireComponent(typeof(MeshCollider))]
[RequireComponent(typeof(MeshFilter))]
public class MarchingCubeMesh : MonoBehaviour
{
    //Mesh shit
    public MeshFilter meshFilter;
    public MeshCollider meshCollider;
    public Mesh mesh;
    public Vector3[] optimizedVerts;
    public int[] optimizedTris;

    //Unity callbacks
    void OnDrawGizmos()
    {
        Gizmos.color = new Color(0, 0, 1, 0.05f);
        Gizmos.DrawCube(transform.position + Vector3.one * 4, new Vector3(8, 8, 8));
    }
    void Awake()
    {
        meshFilter = GetComponent<MeshFilter>();
        meshCollider = GetComponent<MeshCollider>();

        //Inizialize
        mesh = new Mesh();
        meshFilter.sharedMesh = mesh;
    }
}
