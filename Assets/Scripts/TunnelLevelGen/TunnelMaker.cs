using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TunnelMaker : MonoBehaviour
{
    public PointCloudManager PCM;
    private void OnDrawGizmos()
    {
        for (int i = 0; i < SplineNoise3D.SplineLine.Count - 1; i++)
        {
            Debug.DrawLine(SplineNoise3D.SplineLine[i].pos, SplineNoise3D.SplineLine[i + 1].pos, Color.red);
        }
        for (int i = 0; i < SplineNoise3D.SplineLine.Count; i++)
        {
            Gizmos.DrawSphere(SplineNoise3D.SplineLine[i].pos, 0.2f);
        }

    }
    void Start()
    {
        SplineNoise3D.SplineLine = new List<SplineNoise3D.Spline>();
        StartCoroutine(createLevelSLowLike(20, 0.6f, 0.4f));
        PCM.isoSurface = 10f;
    }
    public void makeSpline(int segmentCount, float sporadicFactor,float noiseScale)
    {
        Perlin3D.scale = noiseScale;
        Vector3 direction = Vector3.right;
        Vector3 currentPos = Vector3.one * 20;
        for(int i=0;i< segmentCount; i++)
        {
            float x = Perlin3D.PerlinNoise3D(currentPos + Vector3.right);
            float y = Perlin3D.PerlinNoise3D(currentPos + Vector3.up);
            float z = Perlin3D.PerlinNoise3D(currentPos + Vector3.forward);
            direction = Vector3.Slerp(direction, new Vector3(x, y, z).normalized, sporadicFactor);
            currentPos += direction.normalized * 20f;
            SplineNoise3D.AddSplineSegment(currentPos);
        }    
    }
    public void addOne(Vector3 paintPoint)
    {
        PCM.InitializeIsoSurfaceSphere(paintPoint, 0.1f, SplineNoise3D.SplineNoise);
    }
    public IEnumerator createLevelSLowLike(int segmentCount, float sporadicFactor, float noiseScale)
    {
        makeSpline(segmentCount, sporadicFactor, noiseScale);
        for (int i = 0 ; i < segmentCount; i++)
        {
            addOne(SplineNoise3D.SplineLine[i].pos);
            yield return null;
        }
    }
}
