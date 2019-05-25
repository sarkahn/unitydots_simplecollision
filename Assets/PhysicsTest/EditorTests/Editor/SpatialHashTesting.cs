using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.SceneManagement;

[TestFixture]
public class SpatialHashTesting
{
    List<int> CopyCellList(NativeMultiHashMap<int,int> map, int2 cell)
    {
        List<int> list = new List<int>();
        int hash = cell.GetHashCode();

        //Debug.LogFormat("Retrieving values at cell {0}, hash {1}", cell, hash);

        int item;
        NativeMultiHashMapIterator<int> it;
        if( map.TryGetFirstValue(hash, out item, out it))
        {
            list.Add(item);
            while( map.TryGetNextValue(out item, ref it ))
            {
                list.Add(item);
            }
        }

        return list;
    }

    [Test]
    public void CircleValuesAreInserted()
    {
        var map = new NativeMultiHashMap<int, int>(10, Allocator.Temp);

        float2 pos = new float2(30, 30);
        int cellSize = 10;
        int2 cell = SpatialHashExt.CellFromPos(pos, cellSize);
        int value = 5;
        float radius = 1;

        map.InsertCircle(pos, radius, value, cellSize);

        var items = CopyCellList(map, cell);

        Assert.AreEqual(1, items.Count);
        Assert.AreEqual(value, items[0]);
        
        pos = new float2(14.3f, 7.1f);
        cell = SpatialHashExt.CellFromPos(pos, cellSize);
        value = 15;

        map.InsertCircle(pos, radius, value, cellSize);

        items = CopyCellList(map, cell);

        Assert.AreEqual(1, items.Count);
        Assert.AreEqual(value, items[0]);

        pos = new float2(41f, 55.5f);
        radius = 15;
        value = 45;
        
        map.InsertCircle(pos, radius, value, cellSize);

        // min = 26, 40.5 => 2, 4
        // max = 56, 70.5 => 6, 8

        for ( int x = 2; x < 6; ++x )
        {
            for( int y = 4; y < 8; ++y )
            {
                items = CopyCellList(map, new int2(x,y));

                Assert.AreEqual(1, items.Count);
                Assert.AreEqual(value, items[0]);
            }
        }

    }

    [Test]
    public void CirclesOverlap()
    {
        var map = new NativeMultiHashMap<int, int>(10, Allocator.Temp);

        int valA = 55;
        int radA = 15;

        map.InsertCircle(new float2(10, 10), radA, valA);
        // min = -5, -5 => -1, -1
        // max = 25, 25 => 3, 3

        int valB = 99;
        int radB = 10;
        map.InsertCircle(new float2(30, 10), radB, valB);
        // min = 20, 0 => 2, 0
        // max = 40, 20 => 4, 2

        var values = CopyCellList(map, new int2(2, 1));

        Assert.AreEqual(2, values.Count);
        Assert.Contains(valA, values);
        Assert.Contains(valB, values);

        values = CopyCellList(map, new int2(3, 1));

        Assert.AreEqual(1, values.Count);
        Assert.AreEqual(valB, values[0]);
        
    }

    [Test]
    public void CellsGiveUniqueHashes()
    {
        List<int> hashes = new List<int>();
        int halfRange = 30;
        for( int x = -halfRange; x < halfRange; ++x )
        {
            for( int y = -halfRange; y < halfRange; ++y )
            {
                hashes.Add(new int2(x, y).GetHashCode());
            }
        }

        Assert.AreEqual(hashes.Count, hashes.Distinct().Count());
    }
}
