using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SplineNoise3D
{
    public static List<Spline> SplineLine = new List<Spline>();
    public static float SplineNoise(Vector3 point)
    {
        //OnLine
        for(int i=0;i<SplineLine.Count-1;i++)
        {
            if(IsOnLine(SplineLine[i].pos, SplineLine[i+1].pos, point))
                return LineDistance(SplineLine[i].pos, SplineLine[i + 1].pos, point);
        }
        //InJoint
        for (int i = 1; i < SplineLine.Count-1; i++)
        {
            float hypo = Vector3.Distance(point, SplineLine[i].pos);
            if (hypo < SplineLine[i].radius)
                return (LineDistance(SplineLine[i - 1].pos, SplineLine[i].pos, point) + LineDistance(SplineLine[i].pos, SplineLine[i+1].pos, point)) / 2f;
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
    public static bool IsOnLine(Vector3 lineA, Vector3 lineB, Vector3 pointC)
    {
        float alongLine = distanceOnLine(lineA, lineB, pointC);
        Vector3 BA = lineB - lineA;
        return alongLine > 0f && alongLine <= BA.magnitude;
    }

    //Spline stuff
    public static void AddSplineSegment(Vector3 pos)
    {
        SplineLine.Add(new Spline { pos = pos, radius = 10 });
    }

    public struct Spline
    {
        public Vector3 pos;
        public float radius;
    }
}
