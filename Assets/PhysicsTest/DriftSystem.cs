using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public class DriftSystem : JobComponentSystem
{
    [BurstCompile]
    struct MovementSystemJob : IJobForEach<Translation, Drift, LocalToWorld, Rotation>
    {
        public float dt;
        public void Execute(ref Translation pos, [ReadOnly] ref Drift drift, [ReadOnly] ref LocalToWorld ltw, [ReadOnly] ref Rotation rot)
        {
            var angledDir = Utils.GetAngleDir(pos.Value, ltw.Up, drift.angle);
            float3 p2 = pos.Value + angledDir * drift.speed * dt;
            pos.Value = p2;
        } 
    }
    
    protected override JobHandle OnUpdate(JobHandle inputDependencies)
    {
        var job = new MovementSystemJob
        {
            dt = Time.deltaTime,
        }.Schedule(this, inputDependencies);

        return job;
    }
}