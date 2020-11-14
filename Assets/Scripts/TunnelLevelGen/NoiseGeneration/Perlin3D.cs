using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Perlin3D
{
    public static float scale = 0.2f;
    public static float PerlinNoise3D(Vector3 point)
    {
        return PerlinNoise3D(point.x * scale, point.y * scale, point.z * scale);
    }
    public static float PerlinNoise3D(float x, float y, float z)
    {
        //permutes
        float ab = Mathf.PerlinNoise(x, y);
        float ac = Mathf.PerlinNoise(x, z);
        float bc = Mathf.PerlinNoise(y, z);
        //Inverse
        float ba = Mathf.PerlinNoise(y, x);
        float ca = Mathf.PerlinNoise(z, x);
        float cb = Mathf.PerlinNoise(z, y);
        float abc = ab + ac + bc + ba + ca + cb;
        return abc/6f;
    }
}
