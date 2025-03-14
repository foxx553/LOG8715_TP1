using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[BurstCompile]
public partial struct PlantSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        // Add required components for this system to run
        state.RequireForUpdate<Plant>();
        state.RequireForUpdate<Lifetime>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        // Schedule a job instead of using a foreach loop for better performance
        state.Dependency = new PlantScaleJob().ScheduleParallel(state.Dependency);
    }

    [BurstCompile]
    [WithAll(typeof(Plant))]
    partial struct PlantScaleJob : IJobEntity
    {
        void Execute(RefRO<Lifetime> lifetime, RefRW<LocalTransform> transform)
        {
            // Use float3 instead of Vector3 for better Burst compatibility
            float scale = lifetime.ValueRO._lifetime / lifetime.ValueRO._startingLifetime;
            transform.ValueRW.Scale = scale;
        }
    }
}
