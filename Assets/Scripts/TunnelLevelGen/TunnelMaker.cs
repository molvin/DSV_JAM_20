using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TunnelMaker : MonoBehaviour
{
    public PointCloudManager PCM;
    public float CaveAmount = 4f;
    private void OnDrawGizmos()
    {
        for (int i = 0; i < SplineNoise3D.SplineLine.Count - 1; i++)
        {
            Debug.DrawLine(SplineNoise3D.SplineLine[i].pos, SplineNoise3D.SplineLine[i + 1].pos, Color.red);
        }
        for (int i = 0; i < SplineNoise3D.SplineHole.Count - 1; i++)
        {
            Debug.DrawLine(SplineNoise3D.SplineHole[i].pos, SplineNoise3D.SplineHole[i + 1].pos, Color.green);
        }
        for (int i = 0; i < SplineNoise3D.SplineLine.Count; i++)
        {
            Gizmos.DrawSphere(SplineNoise3D.SplineLine[i].pos, 0.2f);
        }

    }
    public void makeSpline(int segmentCount, float sporadicFactor,float noiseScale)
    {
        Perlin3D.scale = noiseScale;
        Vector3 direction = Vector3.right;
        Vector3 currentPos = Vector3.one * 40;
        for(int i=0;i< segmentCount; i++)
        {
            float x = 0.1f;
            float y = Perlin3D.PerlinNoise3D(currentPos + Vector3.up) - 0.5f;
            float z = Perlin3D.PerlinNoise3D(currentPos + Vector3.forward) - 0.5f;
            direction = Vector3.Slerp(direction, new Vector3(x, y, z).normalized, sporadicFactor);
            currentPos += direction.normalized * 20f;
            SplineNoise3D.AddSplineSegment(currentPos, 10f);
        }    
    }
    public void addOne(Vector3 paintPoint)
    {
        PCM.InitializeIsoSurfaceSphere(paintPoint, 0.1f, SuperNoise);
    }
    public IEnumerator createLevelSLowLike(int segmentCount, float sporadicFactor, float noiseScale)
    {
        SplineNoise3D.SplineLine = new List<SplineNoise3D.Spline>();
        makeSpline(segmentCount, sporadicFactor, noiseScale);
        for (int i = 0 ; i < segmentCount; i++)
        {
            addOne(SplineNoise3D.SplineLine[i].pos);
            yield return null;
        }
    }

    public float SuperNoise(Vector3 point)
    {
        float holesize = 2f;
        //distance along spline X
        float dist = SplineNoise3D.HoleNoise(point)/holesize;
        if (dist > 1f) dist = 1f;
        Perlin3D.scale = 0.2f;
        float caveWalls = SplineNoise3D.SplineNoise(point) + Perlin3D.PerlinNoise3D(point) * CaveAmount;
        if (caveWalls < 10f) caveWalls = 0f;
        return (caveWalls + Perlin3D.PerlinNoise3D(point) * 12f) * dist;
    }

}
