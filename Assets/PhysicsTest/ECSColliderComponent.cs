using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

[Serializable]
public struct ECSCollider : IComponentData
{
    public float radius;
    public CollisionFlags inGroup;
    public CollisionFlags collidesWith;
}

public class ECSColliderComponent : MonoBehaviour, IConvertGameObjectToEntity
{
    [CollisionFlags]
    public CollisionFlags inGroup_;
    [CollisionFlags]
    public CollisionFlags collidesWith_;

    public float radius_ = .5f;
    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, new ECSCollider { radius = radius_, inGroup = inGroup_, collidesWith = collidesWith_ });
    }
}
