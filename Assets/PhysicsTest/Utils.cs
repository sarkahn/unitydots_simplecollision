using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;

public class Utils
{
    public static float3 GetAngleDir(float3 center, float3 fwd, float angle)
    {
        
        var dir = math.normalize(((center + fwd) - center));
        var rot = quaternion.RotateZ(math.radians(angle));
        dir = math.normalize(math.rotate(rot, dir));

        return dir;
    }
    
    public static bool CirclesOverlap(float3 aPos, float aRadius, float3 bPos, float bRadius)
    {
        float r = aRadius + bRadius;
        float dx = bPos.x - aPos.x;
        float dy = aPos.y - bPos.y;

        return dx * dx + dy * dy < r * r;
    }

    public static void VisitGridIndices<TValue>(
    float2 pos, float radius, TValue val,
    System.Action<int, TValue> callback, int cellSize)
    {
        float2 p = pos;
        int2 min = (int2)math.floor((p - radius) / cellSize);
        int2 max = (int2)math.ceil((p + radius) / cellSize);
        int count = max.x - min.x;

        //Debug.LogFormat("Calling visit grid for entity {0}. Pos {1}, Radius {2}. Min {3}, Max {4}. CellSize {5}", val, pos, radius, min, max, cellSize);

        for (int x = min.x; x < max.x; ++x)
        {
            for (int y = min.y; y < max.y; ++y)
            {
                int2 cell = new int2(x, y);
                int hash = cell.GetHashCode();

                //Debug.LogFormat("Inserting entity {0} at {1} hash {2}", val, cell, hash);
                callback(hash, val);
            }
        }
    }
}
