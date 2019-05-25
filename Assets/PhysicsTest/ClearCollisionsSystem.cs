using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Rendering;

[UpdateInGroup(typeof(PresentationSystemGroup))]
[UpdateAfter(typeof(RenderMeshSystemV2))]
public class ClearCollisionsSystem : ComponentSystem
{
    EntityQuery collisionsQuery_;

    protected override void OnCreate()
    {
        collisionsQuery_ = GetEntityQuery(typeof(CollisionTag));
    }

    // Clear any collisions from the previous frame
    protected override void OnUpdate()
    {
        EntityManager.RemoveComponent<CollisionTag>(collisionsQuery_);
        
    }
}