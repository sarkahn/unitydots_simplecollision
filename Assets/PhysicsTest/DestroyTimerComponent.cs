using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

[Serializable]
public struct DestroyTimer : IComponentData
{
    public float value;
}

public class DestroyTimerComponent : MonoBehaviour, IConvertGameObjectToEntity
{
    public float time_ = 10;
    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, new DestroyTimer { value = time_ });

    }
}
