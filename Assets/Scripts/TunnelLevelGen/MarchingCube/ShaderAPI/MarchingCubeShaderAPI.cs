using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class MarchingCubeShaderAPI
{

    private float _isoSurface;
    public float IsoSurface { get { return _isoSurface; } set { _isoSurface = value; marchShader.SetFloat("IsoSurface", _isoSurface); } }
    private ComputeShader marchShader;
    private ComputeBuffer pointCloud_buf;
    private ComputeBuffer vert_buf;
    private ComputeBuffer tris_buf;
    public MarchingCubeShaderAPI(out float[] pointCloud, out Vector3[] verts, out int[] tris)
    {
        //Set up shader
        marchShader = ComputeShader.Instantiate(Resources.Load<ComputeShader>("Shaders/MarchingCubeShader"));
        pointCloud = new float[9 * 9 * 9];
        verts = new Vector3[8 * 8 * 8 * 15];
        tris = new int[8 * 8 * 8 * 15];
        pointCloud_buf = new ComputeBuffer(pointCloud.Length * sizeof(float), sizeof(float), ComputeBufferType.Default);
        vert_buf = new ComputeBuffer(verts.Length * sizeof(float) * 3, sizeof(float) * 3, ComputeBufferType.Default);
        tris_buf = new ComputeBuffer(tris.Length * sizeof(int), sizeof(int), ComputeBufferType.Default);
        marchShader.SetFloat("IsoSurface", 0);
        marchShader.SetBuffer(marchShader.FindKernel("CSMain"), "PointCloud", pointCloud_buf);
        marchShader.SetBuffer(marchShader.FindKernel("CSMain"), "Verts", vert_buf);
        marchShader.SetBuffer(marchShader.FindKernel("CSMain"), "Tris", tris_buf);
    }
    public void MarchCloud(ref float[] pointCloud, ref Vector3[] verts, ref int[] tris)
    {
        pointCloud_buf.SetData(pointCloud);
        marchShader.Dispatch(marchShader.FindKernel("CSMain"), 1, 1, 1);
        vert_buf.GetData(verts);
        tris_buf.GetData(tris);
    }

    public void Release()
    {
        pointCloud_buf.Release();
        vert_buf.Release();
        tris_buf.Release();
    }
}
