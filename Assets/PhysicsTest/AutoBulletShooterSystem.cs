using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using static Unity.Mathematics.math;

public class AutoBulletShooterSystem : JobComponentSystem
{
    EndSimulationEntityCommandBufferSystem initBufferSystem_;
    NativeHashMap<int, Entity> sharedEntityPrefabs_;
    List<BulletSpawner> spawnerTypes_ = new List<BulletSpawner>();
    EntityQuery spawnerQuery_;

    //[BurstCompile]
    struct AutoBulletShooterSystemJob : IJobForEachWithEntity<AutoBulletShooter, LocalToWorld, Rotation>
    {
        [ReadOnly]
        public EntityCommandBuffer.Concurrent commandBuffer;

        [ReadOnly]
        public Entity bulletPrefab;

        public float dt;

        public void Execute(Entity entity, int index, 
            ref AutoBulletShooter shooter, 
            [ReadOnly] ref LocalToWorld ltw, 
            [ReadOnly] ref Rotation rot)
        {
            shooter.shotTimer -= dt;

            if( shooter.shotTimer <= 0 )
            {
                float late = shooter.shotTimer * -1f;
                shooter.shotTimer = shooter.cooldown;

                var e = commandBuffer.Instantiate(index, bulletPrefab);

                var fwd = ltw.Forward;
                var pos = ltw.Position;
                var spawnPos = pos + fwd * late;

                commandBuffer.SetComponent(index, e, new Translation { Value = spawnPos });
                commandBuffer.SetComponent(index, e, rot);
                commandBuffer.SetComponent(index, e, ltw);
                commandBuffer.SetComponent(index, e, new Bullet { spawnedFrom = entity });
                
                //commandBuffer.SetComponent(index, e, ltw);

            }
        }
    }

    protected override void OnCreate()
    {
        initBufferSystem_ = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
        spawnerQuery_ = GetEntityQuery(new EntityQueryDesc
        {
            All = new[] {
                ComponentType.ReadWrite<AutoBulletShooter>(),
                ComponentType.ReadOnly<BulletSpawner>(),
                ComponentType.ReadOnly<LocalToWorld>(),
                ComponentType.ReadOnly<Rotation>(),}
        });
    }

    protected override JobHandle OnUpdate(JobHandle job)
    {
        spawnerTypes_.Clear();

        EntityManager.GetAllUniqueSharedComponentData(spawnerTypes_);

        // Ignore 0 (prefab == Entity.null)
        for( int i = 1; i < spawnerTypes_.Count; ++i )
        {
            spawnerQuery_.SetFilter(spawnerTypes_[i]);

            //Debug.Log("Running shooter job");
            job = new AutoBulletShooterSystemJob
            {
                commandBuffer = initBufferSystem_.CreateCommandBuffer().ToConcurrent(),
                bulletPrefab = spawnerTypes_[i].bulletPrefab,
                dt = Time.deltaTime,
            }.Schedule(spawnerQuery_, job);

            initBufferSystem_.AddJobHandleForProducer(job);

            spawnerQuery_.AddDependency(job);
        }

        return job;
    }
}