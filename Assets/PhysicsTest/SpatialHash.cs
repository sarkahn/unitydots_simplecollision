using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;

public static class SpatialHashExt 
{
    public static void InsertCircle<TValue>(
        this NativeMultiHashMap<int, TValue> map,
        float2 pos, float radius, TValue val, int cellSize = 10)
        where TValue : struct
    {
        float2 p = pos;
        int2 min = (int2)math.floor((p - radius) / cellSize);
        int2 max = (int2)math.ceil((p + radius) / cellSize);
        int count = max.x - min.x;

        //Debug.LogFormat("Inserting circle at {0}, Radius {1}, Min {2}, Max {3}, CellCount {4}", p, radius, min, max, count * count);

        for (int x = min.x; x < max.x; ++x)
        {
            for (int y = min.y; y < max.y; ++y)
            {
                int2 cell = new int2(x, y);
                int hash = cell.GetHashCode();
                //Debug.LogFormat("Inserting at cell {0}, hash {1}", cell, hash);
                map.Add(hash, val);
            }
        }
    }

    public static void VisitGridIndices<TValue>(
        float2 pos, float radius, TValue val,
        System.Action<int, TValue> callback, int cellSize)
    {
        float2 p = pos;
        int2 min = (int2)math.floor((p - radius) / cellSize);
        int2 max = (int2)math.ceil((p + radius) / cellSize);
        int count = max.x - min.x;

        for (int x = min.x; x < max.x; ++x)
        {
            for (int y = min.y; y < max.y; ++y)
            {
                int2 cell = new int2(x, y);
                int hash = cell.GetHashCode();

                callback(hash, val);
            }
        }
    }

    //public static void VisitHashIndices<TValue>(
    //float2 pos, float radius, TValue val, int cellSize = 10, System.Action<int,TValue> callback )
    //{
    //    float2 p = pos;
    //    int2 min = (int2)math.floor((p - radius) / cellSize);
    //    int2 max = (int2)math.ceil((p + radius) / cellSize);
    //    int count = max.x - min.x;

    //    for (int x = min.x; x < max.x; ++x)
    //    {
    //        for (int y = min.y; y < max.y; ++y)
    //        {
    //            int2 cell = new int2(x, y);
    //            int hash = cell.GetHashCode();

    //            callback(hash, val);
    //        }
    //    }
    //}

    public static int2 CellFromPos(float2 pos, int cellSize)
    {
        return (int2)math.floor (pos / (float)cellSize);
    }

}
