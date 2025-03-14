using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[BurstCompile]
public partial struct MovePredatorTowardPreySystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<ConfigComponent>();
        state.RequireForUpdate<Predator>();
        state.RequireForUpdate<Prey>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        // Get config for predator speed
        var config = SystemAPI.GetSingleton<ConfigComponent>();
        float predatorSpeed = config.predatorSpeed;
        
        // Get all prey positions
        var preyQuery = SystemAPI.QueryBuilder().WithAll<Prey, LocalTransform>().Build();
        int preyCount = preyQuery.CalculateEntityCount();
        
        // If no prey, nothing to do
        if (preyCount == 0)
            return;
            
        var preyPositions = new NativeArray<float3>(preyCount, Allocator.TempJob);
        
        // Fill the prey positions array
        {
            int i = 0;
            foreach (var transform in SystemAPI.Query<RefRO<LocalTransform>>().WithAll<Prey>())
            {
                preyPositions[i] = transform.ValueRO.Position;
                i++;
            }
        }
        
        // Schedule the job
        state.Dependency = new MovePredatorTowardPreyJob
        {
            predatorSpeed = predatorSpeed,
            preyPositions = preyPositions
        }.ScheduleParallel(state.Dependency);
        
        // Register disposal of temp arrays
        state.Dependency = preyPositions.Dispose(state.Dependency);
    }

    [BurstCompile]
    [WithAll(typeof(Predator))]
    partial struct MovePredatorTowardPreyJob : IJobEntity
    {
        [ReadOnly] public float predatorSpeed;
        [ReadOnly] public NativeArray<float3> preyPositions;
        
        void Execute(RefRW<Velocity> velocity, RefRO<LocalTransform> transform)
        {
            float3 position = transform.ValueRO.Position;
            
            // Find closest prey
            float closestDistanceSq = float.MaxValue;
            float3 closestPosition = position;
            
            for (int i = 0; i < preyPositions.Length; i++)
            {
                float distanceSq = math.distancesq(preyPositions[i], position);
                if (distanceSq < closestDistanceSq)
                {
                    closestDistanceSq = distanceSq;
                    closestPosition = preyPositions[i];
                }
            }
            
            // Calculate direction and set velocity
            if (closestDistanceSq < float.MaxValue)
            {
                float3 direction = closestPosition - position;
                velocity.ValueRW.velocity = direction * predatorSpeed;
            }
        }
    }
}
