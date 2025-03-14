using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[BurstCompile]
public partial struct MovePreyTowardPlantSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<ConfigComponent>();
        state.RequireForUpdate<Prey>();
        state.RequireForUpdate<Plant>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        // Get config for prey speed
        var config = SystemAPI.GetSingleton<ConfigComponent>();
        float preySpeed = config.preySpeed;
        
        // Get all plant positions
        var plantQuery = SystemAPI.QueryBuilder().WithAll<Plant, LocalTransform>().Build();
        int plantCount = plantQuery.CalculateEntityCount();
        
        // If no plants, nothing to do
        if (plantCount == 0)
            return;
            
        var plantPositions = new NativeArray<float3>(plantCount, Allocator.TempJob);
        
        // Fill the plant positions array
        {
            int i = 0;
            foreach (var transform in SystemAPI.Query<RefRO<LocalTransform>>().WithAll<Plant>())
            {
                plantPositions[i] = transform.ValueRO.Position;
                i++;
            }
        }
        
        // Schedule the job
        state.Dependency = new MovePreyTowardPlantJob
        {
            preySpeed = preySpeed,
            plantPositions = plantPositions
        }.ScheduleParallel(state.Dependency);
        
        // Register disposal of temp arrays
        state.Dependency = plantPositions.Dispose(state.Dependency);
    }

    [BurstCompile]
    [WithAll(typeof(Prey))]
    partial struct MovePreyTowardPlantJob : IJobEntity
    {
        [ReadOnly] public float preySpeed;
        [ReadOnly] public NativeArray<float3> plantPositions;
        
        void Execute(RefRW<Velocity> velocity, RefRO<LocalTransform> transform)
        {
            float3 position = transform.ValueRO.Position;
            
            // Find closest plant
            float closestDistanceSq = float.MaxValue;
            float3 closestPosition = position;
            
            for (int i = 0; i < plantPositions.Length; i++)
            {
                float distanceSq = math.distancesq(plantPositions[i], position);
                if (distanceSq < closestDistanceSq)
                {
                    closestDistanceSq = distanceSq;
                    closestPosition = plantPositions[i];
                }
            }
            
            // Calculate direction and set velocity
            if (closestDistanceSq < float.MaxValue)
            {
                float3 direction = closestPosition - position;
                velocity.ValueRW.velocity = direction * preySpeed;
            }
        }
    }
}
