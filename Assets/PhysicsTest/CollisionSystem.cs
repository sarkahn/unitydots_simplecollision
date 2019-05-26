using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

//[DisableAutoCreation]
public class CollisionSystem : JobComponentSystem
{
    NativeMultiHashMap<int, Entity> spatialMap_;
    NativeList<int> keysList_;

    static readonly int CellSize = 20;

    EndSimulationEntityCommandBufferSystem initBufferSystem_;

    [BurstCompile]
    struct BuildSpatialMap : IJobForEachWithEntity<Translation, ECSCollider>
    {
        public NativeMultiHashMap<int, Entity>.Concurrent spatialMap;

        public void Execute(Entity e, int chunkIndex, [ReadOnly] ref Translation translation, [ReadOnly] ref ECSCollider coll)
        {
            float2 p = translation.Value.xy;
            
            Utils.VisitGridIndices(p, coll.radius, e, spatialMap.Add, CellSize);
        }
    }
    
    [BurstCompile]
    struct InitializeKeysList : IJob
    {
        public NativeList<int> outKeys;

        [ReadOnly]
        public NativeMultiHashMap<int, Entity> spatialMap;

        public void Execute()
        {
            

            var keys = spatialMap.GetKeyArray(Allocator.Temp);

            if (keys.Length == 0)
                return;
            
            outKeys.Add(keys[0]);
            int lastKey = keys[0];

            for ( int i = 0; i < keys.Length; ++i )
            {
                if (lastKey == keys[i] || outKeys.Contains(keys[i]))
                    continue;
                lastKey = keys[i];
                outKeys.Add(keys[i]);
            }
            
        }
    };

    //[BurstCompile]
    struct GenerateCollisionData : IJobParallelForDefer
    {
        [ReadOnly]
        public NativeMultiHashMap<int, Entity> spatialMap;

        [ReadOnly]
        public NativeArray<int> keys;

        [ReadOnly]
        public ComponentDataFromEntity<ECSCollider> colliderFromEntity;

        [ReadOnly]
        public ComponentDataFromEntity<Translation> posFromEntity;

        public EntityCommandBuffer.Concurrent commandBuffer;
        
        public void Execute(int index)
        {
            NativeList<Entity> entities = new NativeList<Entity>(Allocator.Temp);

            NativeMultiHashMapIterator<int> it;

            

            Entity curr;
            if( spatialMap.TryGetFirstValue(keys[index], out curr, out it ))
            {
                entities.Add(curr);
                Entity next;
                while (spatialMap.TryGetNextValue(out next, ref it))
                {
                    var aColl = colliderFromEntity[curr];
                    var bColl = colliderFromEntity[next];
                    var a = posFromEntity[curr].Value;
                    var b = posFromEntity[next].Value;

                    entities.Add(next);
                }
            }

            for( int i = 0; i < entities.Length; ++i )
            {
                for( int j = i + 1; j < entities.Length; ++j )
                {
                    var a = entities[i];
                    var b = entities[j];

                    var aColl = colliderFromEntity[a];
                    var bColl = colliderFromEntity[b];
                    var aPos = posFromEntity[a].Value;
                    var bPos = posFromEntity[b].Value;

                    if (Utils.Collides(aColl.collidesWith, bColl.inGroup))
                    {
                        if (Utils.CirclesOverlap(aPos, aColl.radius, bPos, bColl.radius))
                        {
                            commandBuffer.AddComponent(index, a, new CollisionTag());
                            commandBuffer.AddComponent(index, b, new CollisionTag());
                        }
                    }

                }
            }
        }
    }
    
    protected override void OnCreate()
    {
        spatialMap_ = new NativeMultiHashMap<int, Entity>(5000, Allocator.Persistent);
        keysList_ = new NativeList<int>(Allocator.Persistent);
        initBufferSystem_ = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
    }

    protected override void OnDestroy()
    {
        spatialMap_.Dispose();
        keysList_.Dispose();
        
    }

    protected override JobHandle OnUpdate(JobHandle inputDependencies)
    {
        var job = inputDependencies;

        spatialMap_.Clear();
        keysList_.Clear();

        // Build our spatial map
        job = new BuildSpatialMap
        {
            spatialMap = spatialMap_.ToConcurrent(),
        }.Schedule(this, job);

        // Initialize the size of our keys list. We can't know the size of our list
        // during schedule time so we need to use "DeferredJobArray"
        // Example in Packages/Jobs/Unity.Jobs.Test/NativeListDeferredArrayTests
        job = new InitializeKeysList
        {
            outKeys = keysList_,
            spatialMap = spatialMap_,
        }.Schedule(job);

        // Check for collisions and tag entities
        job = new GenerateCollisionData
        {
            spatialMap = spatialMap_,
            keys = keysList_.AsDeferredJobArray(),
            colliderFromEntity = GetComponentDataFromEntity<ECSCollider>(true),
            posFromEntity = GetComponentDataFromEntity<Translation>(true),
            commandBuffer = initBufferSystem_.CreateCommandBuffer().ToConcurrent(),
        }.Schedule(keysList_, 5, job);

        return job;
    }
}