using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

[Serializable]
[InternalBufferCapacity(4)]
public struct CollisionBuffer : IBufferElementData
{
    public Entity value;
    public static implicit operator Entity(CollisionBuffer col) { return col.value; }
    public static implicit operator CollisionBuffer(Entity e) { return new CollisionBuffer { value = e }; }
}

public class CollisionBufferComponent : MonoBehaviour, IConvertGameObjectToEntity
{
    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddBuffer<CollisionBuffer>(entity);
    }
}