using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SplineNoise3D
{
    public static List<Spline> SplineLine = new List<Spline>();
    public static List<Spline> SplineHole = new List<Spline>();
    public static float SplineNoise(Vector3 point)
    {
        return SplineDistance(point);
    }
    public static float SplineDistance(Vector3 point)
    {
        float smaletsFound = float.MaxValue;
        //OnLine
        for (int i = 0; i < SplineLine.Count - 1; i++)
        {
            if (IsOnLine(SplineLine[i].pos, SplineLine[i + 1].pos, point))
            {
                float value = LineDistance(SplineLine[i].pos, SplineLine[i + 1].pos, point);
                if (value < smaletsFound)
                    smaletsFound = value;
            } 
        }
        if (smaletsFound != float.MaxValue)
            return smaletsFound;

        //InJoint
        for (int i = 1; i < SplineLine.Count - 1; i++)
        {
            float hypo = Vector3.Distance(point, SplineLine[i].pos);
            if (hypo < SplineLine[i].radius)
                return (LineDistance(SplineLine[i - 1].pos, SplineLine[i].pos, point) + LineDistance(SplineLine[i].pos, SplineLine[i + 1].pos, point)) / 2f;
        }
        return 100;
    }
    public static float HoleNoise(Vector3 point)
    {
        float smaletsFound = float.MaxValue;
        //OnLine
        for (int i = 0; i < SplineHole.Count - 1; i++)
        {
            if (IsOnLine(SplineHole[i].pos, SplineHole[i + 1].pos, point))
            {
                float value = LineDistance(SplineHole[i].pos, SplineHole[i + 1].pos, point);
                if (value < smaletsFound)
                    smaletsFound = value;
            }
        }
        if (smaletsFound != float.MaxValue)
            return smaletsFound;
        //InJoint
        for (int i = 1; i < SplineHole.Count - 1; i++)
        {
            float hypo = Vector3.Distance(point, SplineHole[i].pos);
            if (hypo < SplineHole[i].radius)
                return (LineDistance(SplineHole[i - 1].pos, SplineHole[i].pos, point) + LineDistance(SplineHole[i].pos, SplineHole[i + 1].pos, point)) / 2f;
        }
        return 100;
    }
    public static float LineDistance(Vector3 lineA, Vector3 lineB, Vector3 pointC)
    {
        Vector3 AC = pointC - lineB;
        Vector3 AB = lineB - lineA;
        return Vector3.Cross(AC, AB).magnitude / AB.magnitude;
    }
    public static float distanceOnLine(Vector3 lineA, Vector3 lineB, Vector3 pointC)
    {
        Vector3 CA = pointC - lineA;
        Vector3 BA = lineB - lineA;
        return Vector3.Dot(CA, BA.normalized);
    }
    public static float distanceOnSpline(Vector3 pointC)
    {
        float total = 0f;
        for (int i = 0; i < SplineLine.Count - 1; i++)
        {
            float maxDist = (SplineLine[i].pos - SplineLine[i + 1].pos).magnitude;
            float dis = distanceOnLine(SplineLine[i].pos, SplineLine[i + 1].pos, pointC);
            if (dis > maxDist)
            {
                total += maxDist;
            }
            else
            {
                total += dis;
                return total;
            }
        }
        return 0f;
    }
    public static Vector3 getPointOnSpline(Vector3 pointC)
    {
        for (int i = 0; i < SplineLine.Count - 1; i++)
        {
            if (IsOnLine(SplineLine[i].pos, SplineLine[i + 1].pos, pointC))
            {
                float dis = LineDistance(SplineLine[i].pos, SplineLine[i + 1].pos, pointC);
                return SplineLine[i].pos + (SplineLine[i + 1].pos - SplineLine[i].pos).normalized * dis;
            }
        }
        //InJoint
        for (int i = 0; i < SplineLine.Count - 1; i++)
        {
            float maxDist = (SplineLine[i+1].pos - SplineLine[i].pos).magnitude;
            float dis = distanceOnLine(SplineLine[i].pos, SplineLine[i + 1].pos, pointC);
            if (dis <= maxDist)
            {
                return SplineLine[i].pos;
            }
        }
        return SplineLine[SplineLine.Count-1].pos;
    }
    public static bool IsOnLine(Vector3 lineA, Vector3 lineB, Vector3 pointC)
    {
        float alongLine = distanceOnLine(lineA, lineB, pointC);
        Vector3 BA = lineB - lineA;
        return alongLine > 0f && alongLine <= BA.magnitude;
    }
    //Spline stuff
    public static void AddSplineSegment(Vector3 pos, float holeOffsetRadius)
    {
        SplineLine.Add(new Spline { pos = pos, radius = 10 });
        //Hole!
        float x = (Perlin3D.PerlinNoise3D(pos + Vector3.up) -0.5f) * 2 * holeOffsetRadius;
        float y = (Perlin3D.PerlinNoise3D(pos + Vector3.right) - 0.5f) *2 * holeOffsetRadius;
        float z = (Perlin3D.PerlinNoise3D(pos + Vector3.forward) - 0.5f) *2 * holeOffsetRadius;
        Vector3 off = new Vector3(x, y, z);
        SplineHole.Add(new Spline { pos = pos + off, radius = 5f });
    }
    public struct Spline
    {
        public Vector3 pos;
        public float radius;
    }
}
