using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using static ECSComponents;

[BurstCompile]
public partial struct PlantScalingSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        // Process plants
        foreach (var (scale, lifetime, transform) in SystemAPI.Query<RefRW<ScaleComponent>, RefRO<LifetimeData>, RefRW<LocalTransform>>().WithAll<PlantTag>())
        {
            // Update scale based on lifetime progression
            scale.ValueRW.Value = lifetime.ValueRO.CurrentLifetime / lifetime.ValueRO.StartingLifetime;
            transform.ValueRW.Scale = scale.ValueRO.Value;
        }
    }
}