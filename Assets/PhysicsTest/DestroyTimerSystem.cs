using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using static Unity.Mathematics.math;

public class DestroyTimerSystem : JobComponentSystem
{
    EndSimulationEntityCommandBufferSystem initBufferSystem_;

    struct DestroyTimerSystemJob : IJobForEachWithEntity<DestroyTimer>
    {
        [ReadOnly]
        public EntityCommandBuffer.Concurrent commandBuffer;
        
        public float dt;
        public void Execute(Entity e, int index, ref DestroyTimer c0)
        {
            c0.value -= dt;

            if( c0.value <= 0)
            {
                commandBuffer.DestroyEntity(index, e);
            }
        }
    }

    protected override void OnCreate()
    {
        initBufferSystem_ = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
    }

    protected override JobHandle OnUpdate(JobHandle job)
    {
        job = new DestroyTimerSystemJob
        {
            dt = Time.deltaTime,
            commandBuffer = initBufferSystem_.CreateCommandBuffer().ToConcurrent(),
        }.Schedule(this, job);

        initBufferSystem_.AddJobHandleForProducer(job);

        return job;
    }
}