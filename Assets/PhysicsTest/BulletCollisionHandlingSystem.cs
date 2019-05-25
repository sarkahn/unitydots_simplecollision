using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using Unity.Rendering;

[UpdateInGroup(typeof(PresentationSystemGroup))]
[UpdateBefore(typeof(RenderMeshSystemV2))]
public class BulletCollisionHandlingSystem : ComponentSystem
{
    EntityQuery bulletCollisionQuery_;

    protected override void OnCreate()
    {
        bulletCollisionQuery_ = GetEntityQuery(typeof(CollisionTag), typeof(Bullet));
    }

    protected override void OnUpdate()
    {
        EntityManager.DestroyEntity(bulletCollisionQuery_);
        EntityManager.RemoveComponent<CollisionTag>(bulletCollisionQuery_);
    }
}