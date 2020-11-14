using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Rendering;

public class PaintPointCloudShaderAPI
{
    private ComputeShader paintShader;
    private ComputeBuffer pointCloud_buf;
    public PaintPointCloudShaderAPI(Vector3 canvasWorldPosition)
    {
        //Set up shader
        paintShader = ComputeShader.Instantiate(Resources.Load<ComputeShader>("Shaders/PaintPointCloudShader"));
        paintShader.SetFloat("BrushAmount", 0);
        paintShader.SetFloat("BrushRadius", 1);
        paintShader.SetVector("BrushPosition", Vector3.zero);
        paintShader.SetVector("WorldPos", canvasWorldPosition);
        pointCloud_buf = new ComputeBuffer(8 * 8 * 8 * sizeof(float), sizeof(float), ComputeBufferType.Default);
        paintShader.SetBuffer(paintShader.FindKernel("CSMain"), "PointCloud", pointCloud_buf);
    }
    public void MoveCanvas(Vector3 canvasWorldPosition)
    {
        paintShader.SetVector("WorldPos", canvasWorldPosition);
    }
    public void PaintCloud(ref float[] pointCloud, Vector3 brushPos, float brushRadius, float amount)
    {
        pointCloud_buf.SetData(pointCloud);
        paintShader.SetVector("BrushPosition", brushPos);
        paintShader.SetFloat("BrushRadius", brushRadius);
        paintShader.SetFloat("BrushAmount", amount);
        paintShader.Dispatch(paintShader.FindKernel("CSMain"), 1, 1, 1);
        pointCloud_buf.GetData(pointCloud);
    }

    public void PaintCloudAsync(Action<AsyncGPUReadbackRequest> callback, ref float[] pointCloud, Vector3 brushPos, float brushRadius, float amount)
    {
        pointCloud_buf.SetData(pointCloud);
        paintShader.SetVector("BrushPosition", brushPos);
        paintShader.SetFloat("BrushRadius", brushRadius);
        paintShader.SetFloat("BrushAmount", amount);
        paintShader.Dispatch(paintShader.FindKernel("CSMain"), 1, 1, 1);
        AsyncGPUReadback.Request(pointCloud_buf, callback);
    }
    public void Release()
    {
        pointCloud_buf.Release();
    }
}
