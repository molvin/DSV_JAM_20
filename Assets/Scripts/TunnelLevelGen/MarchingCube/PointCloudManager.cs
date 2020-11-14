using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointCloudManager : MonoBehaviour
{
    public MarchingCubeMesh Prefab8X8;

    public float isoSurface;
    private Dictionary<Vector3Int, MarchingCubeMesh> meshGrid = new Dictionary<Vector3Int, MarchingCubeMesh>();

    //Shader shit
    MarchingCubeShaderAPI marchShader;
    private float[] pointCloud;
    private Vector3[] verts;
    private int[] tris;

    public void Awake()
    {
        marchShader = new MarchingCubeShaderAPI(out pointCloud, out verts, out tris);
        marchShader.IsoSurface = isoSurface;
    }
    public void InitializeIsoSurfaceSphere(Vector3 brushPoint, float brushRadius, Func<Vector3, float> initDef)
    {
        int halfExtend = Mathf.FloorToInt(brushRadius / 8f) + 2;
        Vector3Int Chunk = new Vector3Int((int)(brushPoint.x / 8f), (int)(brushPoint.y / 8f), (int)(brushPoint.z / 8f));

        for (int x = -halfExtend; x <= halfExtend; x++)
            for (int y = -halfExtend; y <= halfExtend; y++)
                for (int z = -halfExtend; z <= halfExtend; z++)
                {
                    Vector3Int chuckID = Chunk + new Vector3Int(x, y, z);
                    if (!meshGrid.ContainsKey(chuckID))
                    {
                        MarchingCubeMesh mesh = Instantiate(Prefab8X8, chuckID * 8, Quaternion.identity, transform);
                        meshGrid.Add(chuckID, mesh);
                        SetPointCloud(initDef, new Vector3(chuckID.x, chuckID.y, chuckID.z) * 8f);
                        marchShader.MarchCloud(ref pointCloud, ref verts, ref tris);
                        CleanVerts(mesh);
                    }
                }
    }
    private void CleanVerts(MarchingCubeMesh current)
    {
        int lastRealIndex = 0;
        for (int i = 0; i < verts.Length; i++)
        {
            if (verts[i].x >= 0)
            {
                verts[lastRealIndex] = verts[i];
                lastRealIndex++;
            }
        }

        //Make smaller
        current.optimizedVerts = new Vector3[lastRealIndex];
        current.optimizedTris = new int[lastRealIndex];
        Array.Copy(verts, current.optimizedVerts, lastRealIndex);
        Array.Copy(tris, current.optimizedTris, lastRealIndex);

        //Apply mesh
        current.meshFilter.sharedMesh.Clear();
        current.meshFilter.sharedMesh.vertices = current.optimizedVerts;
        current.meshFilter.sharedMesh.triangles = current.optimizedTris;
        current.meshFilter.sharedMesh.RecalculateNormals();
        current.meshCollider.sharedMesh = current.meshFilter.sharedMesh;
    }
    public void SetPointCloud(Func<Vector3, float> initDef, Vector3 pos)
    {
        for (int z = 0; z < 9; z++)
            for (int y = 0; y < 9; y++)
                for (int x = 0; x < 9; x++)
                {
                    int id = x + ((8 + 1) * y) + ((8 + 1) * (8 + 1) * z);
                    pointCloud[id] = initDef.Invoke(pos + new Vector3(x, y, z));
                }
    }
    public void OnDestroy()
    {
        marchShader.Release();
    }
}
