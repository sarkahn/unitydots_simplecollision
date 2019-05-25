using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Collections;
using System;

[Serializable]
public struct BulletSpawner : ISharedComponentData
{
    public Entity bulletPrefab;
}

public class BulletSpawnerSharedComponent : MonoBehaviour, IConvertGameObjectToEntity, IDeclareReferencedPrefabs
{
    public GameObject pfb_Bullet_;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        var entityPrefab = conversionSystem.GetPrimaryEntity(pfb_Bullet_);
        dstManager.AddSharedComponentData(entity, new BulletSpawner { bulletPrefab = entityPrefab });
    }

    public void DeclareReferencedPrefabs(List<GameObject> referencedPrefabs)
    {
        referencedPrefabs.Add(pfb_Bullet_);
    }
}
