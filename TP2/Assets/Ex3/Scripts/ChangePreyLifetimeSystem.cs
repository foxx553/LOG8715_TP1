using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[BurstCompile]
public partial struct ChangePreyLifetimeSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<ConfigComponent>();
        state.RequireForUpdate<Prey>();
        state.RequireForUpdate<Lifetime>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        // Get config for touching distance
        var config = SystemAPI.GetSingleton<ConfigComponent>();
        float touchingDistanceSq = config.touchingDistance * config.touchingDistance;
        
        // These sizes change every frame as entities are created/destroyed
        var plantQuery = SystemAPI.QueryBuilder().WithAll<Plant, LocalTransform>().Build();
        var plantCount = plantQuery.CalculateEntityCount();
        var plantPositions = new NativeArray<float3>(plantCount, Allocator.TempJob);
        
        var predatorQuery = SystemAPI.QueryBuilder().WithAll<Predator, LocalTransform>().Build();
        var predatorCount = predatorQuery.CalculateEntityCount();
        var predatorPositions = new NativeArray<float3>(predatorCount, Allocator.TempJob);
        
        var preyQuery = SystemAPI.QueryBuilder().WithAll<Prey, LocalTransform>().Build();
        var preyCount = preyQuery.CalculateEntityCount();
        var preyPositions = new NativeArray<float3>(preyCount, Allocator.TempJob);
        var preyEntities = new NativeArray<Entity>(preyCount, Allocator.TempJob);
        
        // Fill the position arrays
        {
            int i = 0;
            foreach (var transform in SystemAPI.Query<RefRO<LocalTransform>>().WithAll<Plant>())
            {
                plantPositions[i] = transform.ValueRO.Position;
                i++;
            }
            
            i = 0;
            foreach (var transform in SystemAPI.Query<RefRO<LocalTransform>>().WithAll<Predator>())
            {
                predatorPositions[i] = transform.ValueRO.Position;
                i++;
            }
            
            i = 0;
            foreach (var (transform, entity) in 
                     SystemAPI.Query<RefRO<LocalTransform>>()
                             .WithAll<Prey>()
                             .WithEntityAccess())
            {
                preyPositions[i] = transform.ValueRO.Position;
                preyEntities[i] = entity;
                i++;
            }
        }

        // Schedule the job to process all prey
        state.Dependency = new ChangePreyLifetimeJob
        {
            touchingDistanceSq = touchingDistanceSq,
            plantPositions = plantPositions,
            predatorPositions = predatorPositions,
            preyPositions = preyPositions,
            preyEntities = preyEntities
        }.ScheduleParallel(state.Dependency);

        // Register disposal of temp arrays
        state.Dependency = plantPositions.Dispose(state.Dependency);
        state.Dependency = predatorPositions.Dispose(state.Dependency);
        state.Dependency = preyPositions.Dispose(state.Dependency);
        state.Dependency = preyEntities.Dispose(state.Dependency);
    }

    [BurstCompile]
    [WithAll(typeof(Prey))]
    partial struct ChangePreyLifetimeJob : IJobEntity
    {
        [ReadOnly] public float touchingDistanceSq;
        [ReadOnly] public NativeArray<float3> plantPositions;
        [ReadOnly] public NativeArray<float3> predatorPositions;
        [ReadOnly] public NativeArray<float3> preyPositions;
        [ReadOnly] public NativeArray<Entity> preyEntities;

        void Execute(RefRW<Lifetime> lifetime, RefRO<LocalTransform> transform, Entity entity)
        {
            // Set default decreasing factor
            lifetime.ValueRW.decreasingFactor = 1.0f;
            
            float3 position = transform.ValueRO.Position;
            
            // Check for proximity to plants
            for (int i = 0; i < plantPositions.Length; i++)
            {
                float distSq = math.distancesq(plantPositions[i], position);
                if (distSq < touchingDistanceSq)
                {
                    lifetime.ValueRW.decreasingFactor /= 2f;
                    break;
                }
            }
            
            // Check for proximity to predators
            for (int i = 0; i < predatorPositions.Length; i++)
            {
                float distSq = math.distancesq(predatorPositions[i], position);
                if (distSq < touchingDistanceSq)
                {
                    lifetime.ValueRW.decreasingFactor *= 2f;
                    break;
                }
            }
            
            // Check for proximity to other prey
            for (int i = 0; i < preyPositions.Length; i++)
            {
                // Skip self
                if (preyEntities[i].Index == entity.Index) continue;
                
                float distSq = math.distancesq(preyPositions[i], position);
                if (distSq < touchingDistanceSq)
                {
                    lifetime.ValueRW.reproduced = true;
                    break;
                }
            }
        }
    }
}
