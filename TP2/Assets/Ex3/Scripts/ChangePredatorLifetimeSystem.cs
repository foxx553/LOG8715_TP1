using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[BurstCompile]
public partial struct ChangePredatorLifetimeSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<ConfigComponent>();
        state.RequireForUpdate<Predator>();
        state.RequireForUpdate<Lifetime>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        // Get config for touching distance
        var config = SystemAPI.GetSingleton<ConfigComponent>();
        float touchingDistanceSq = config.touchingDistance * config.touchingDistance;
        
        // Store positions of all predators in a NativeArray
        var predatorQuery = SystemAPI.QueryBuilder().WithAll<Predator, LocalTransform>().Build();
        var predatorCount = predatorQuery.CalculateEntityCount();
        var predatorPositions = new NativeArray<float3>(predatorCount, Allocator.TempJob);
        var predatorEntities = new NativeArray<Entity>(predatorCount, Allocator.TempJob);
        
        // Store positions of all prey in a NativeArray
        var preyQuery = SystemAPI.QueryBuilder().WithAll<Prey, LocalTransform>().Build();
        var preyCount = preyQuery.CalculateEntityCount();
        var preyPositions = new NativeArray<float3>(preyCount, Allocator.TempJob);
        
        // Fill the position arrays
        {
            int i = 0;
            foreach (var (transform, entity) in SystemAPI.Query<RefRO<LocalTransform>>()
                                                       .WithAll<Predator>()
                                                       .WithEntityAccess())
            {
                predatorPositions[i] = transform.ValueRO.Position;
                predatorEntities[i] = entity;
                i++;
            }

            i = 0;
            foreach (var transform in SystemAPI.Query<RefRO<LocalTransform>>().WithAll<Prey>())
            {
                preyPositions[i] = transform.ValueRO.Position;
                i++;
            }
        }

        // Schedule the job to process all predators
        state.Dependency = new ChangePredatorLifetimeJob
        {
            touchingDistanceSq = touchingDistanceSq,
            predatorPositions = predatorPositions,
            predatorEntities = predatorEntities,
            preyPositions = preyPositions
        }.ScheduleParallel(state.Dependency);

        // Register disposal of temp arrays
        state.Dependency = predatorPositions.Dispose(state.Dependency);
        state.Dependency = predatorEntities.Dispose(state.Dependency);
        state.Dependency = preyPositions.Dispose(state.Dependency);
    }

    [BurstCompile]
    partial struct ChangePredatorLifetimeJob : IJobEntity
    {
        [ReadOnly] public float touchingDistanceSq;
        [ReadOnly] public NativeArray<float3> predatorPositions;
        [ReadOnly] public NativeArray<Entity> predatorEntities;
        [ReadOnly] public NativeArray<float3> preyPositions;

        void Execute(RefRW<Lifetime> lifetime, RefRO<LocalTransform> transform, Entity entity)
        {
            // Set default decreasing factor
            lifetime.ValueRW.decreasingFactor = 1.0f;
            
            float3 position = transform.ValueRO.Position;
            
            // Check for proximity to other predators
            for (int i = 0; i < predatorPositions.Length; i++)
            {
                // Skip self
                if (predatorEntities[i].Index == entity.Index) continue;
                
                float distSq = math.distancesq(predatorPositions[i], position);
                if (distSq < touchingDistanceSq)
                {
                    lifetime.ValueRW.reproduced = true;
                    break;
                }
            }
            
            // Check for proximity to prey
            for (int i = 0; i < preyPositions.Length; i++)
            {
                float distSq = math.distancesq(preyPositions[i], position);
                if (distSq < touchingDistanceSq)
                {
                    lifetime.ValueRW.decreasingFactor /= 2f;
                    break;
                }
            }
        }
    }
}
