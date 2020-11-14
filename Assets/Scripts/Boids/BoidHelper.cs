using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class BoidHelper
{
    const int NumViewDirections = 200;
    public static readonly Vector3[] Directions;

    static BoidHelper()
    {
        Directions = new Vector3[NumViewDirections];

        float GoldenRatio = (1 + Mathf.Sqrt(5)) / 2;
        float AngleIncrement = Mathf.PI * 2 * GoldenRatio;

        for (int i = 0; i < NumViewDirections; i++)
        {
            float t = (float)i / NumViewDirections;
            float Inclination = Mathf.Acos(1 - 2 *t);
            float Azimuth = AngleIncrement * i;

            float x = Mathf.Sin(Inclination) * Mathf.Cos(Azimuth);
            float y = Mathf.Sin(Inclination) * Mathf.Sin(Azimuth);
            float z = Mathf.Cos(Inclination);
            Directions[i] = new Vector3(x, y, z);
        }
    }
}
