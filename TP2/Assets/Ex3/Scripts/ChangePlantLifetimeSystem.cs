using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[BurstCompile]
public partial struct ChangePlantLifetimeSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        // Only run this system when we have plants and config data
        state.RequireForUpdate<ConfigComponent>();
        state.RequireForUpdate<Plant>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        // Get touching distance from config
        var config = SystemAPI.GetSingleton<ConfigComponent>();
        float touchingDistanceSq = config.touchingDistance * config.touchingDistance;
        
        // Get prey positions - using proper component type syntax
        var preyQuery = SystemAPI.QueryBuilder()
                                .WithAll<Prey>()
                                .WithAll<LocalTransform>()
                                .Build();
        int preyCount = preyQuery.CalculateEntityCount();
        
        // If no prey, nothing to do
        if (preyCount == 0)
            return;
            
        var preyPositions = new NativeArray<float3>(preyCount, Allocator.TempJob);
        
        // Fill the prey positions array with consistent component query syntax
        {
            int i = 0;
            foreach (var transform in SystemAPI.Query<RefRO<LocalTransform>>()
                                             .WithAll<Prey>())
            {
                preyPositions[i] = transform.ValueRO.Position;
                i++;
            }
        }
        
        // Schedule the job
        state.Dependency = new ChangePlantLifetimeJob
        {
            touchingDistanceSq = touchingDistanceSq,
            preyPositions = preyPositions
        }.ScheduleParallel(state.Dependency);
        
        // Register disposal of temp arrays
        state.Dependency = preyPositions.Dispose(state.Dependency);
    }

    [BurstCompile]
    [WithAll(typeof(Plant))]
    [WithAll(typeof(Lifetime))]
    [WithAll(typeof(LocalTransform))]
    partial struct ChangePlantLifetimeJob : IJobEntity
    {
        [ReadOnly] public float touchingDistanceSq;
        [ReadOnly] public NativeArray<float3> preyPositions;
        
        void Execute(RefRW<Lifetime> lifetime, RefRO<LocalTransform> transform)
        {
            // Set default decreasing factor
            lifetime.ValueRW.decreasingFactor = 1.0f;
            
            float3 position = transform.ValueRO.Position;
            
            // Check for proximity to prey
            for (int i = 0; i < preyPositions.Length; i++)
            {
                float distSq = math.distancesq(preyPositions[i], position);
                if (distSq < touchingDistanceSq)
                {
                    lifetime.ValueRW.decreasingFactor *= 2f;
                    break;
                }
            }
        }
    }
}
