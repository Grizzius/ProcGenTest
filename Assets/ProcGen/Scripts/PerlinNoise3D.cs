using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PerlinNoise3D
{
    public static float Perlin3D(float x, float y, float z, Vector3 offset)
    {
        float AB = Mathf.PerlinNoise(x + offset.x, y + offset.y);
        float BC = Mathf.PerlinNoise(y + offset.y, z + offset.z);

        float BA = Mathf.PerlinNoise(y + offset.y, x + offset.x);
        float CB = Mathf.PerlinNoise(z + offset.z, y + offset.y);
        float CA = Mathf.PerlinNoise(z + offset.z, x + offset.x);

        float ABC = AB + BC + BA + CB + CA;
        return ABC / 6f;
    }

    
}
