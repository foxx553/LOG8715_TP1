using Unity.Burst;
using Unity.Entities;
using Unity.Collections;
using UnityEngine;

[BurstCompile]
public partial struct LifetimeSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<Lifetime>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        // We use the singleton ECB approach here because:
        // 1. We need a ParallelWriter for thread safety in jobs
        // 2. ECB playback is scheduled automatically after our system runs
        var ecbSingleton = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>();
        var ecbParallel = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged).AsParallelWriter();
        
        // Schedule the job and update dependency chain
        // (removed .Complete() to allow async execution)
        state.Dependency = new LifetimeJob
        {
            deltaTime = SystemAPI.Time.DeltaTime,
            ecb = ecbParallel
        }.ScheduleParallel(state.Dependency);
    }

    // IJobEntity automatically iterates through all entities with the specified components
    [BurstCompile]
    partial struct LifetimeJob : IJobEntity
    {
        public float deltaTime;
        public EntityCommandBuffer.ParallelWriter ecb;
        
        // This method will be called once per matching entity
        void Execute(RefRW<Lifetime> lifetime, Entity entity, [EntityIndexInQuery] int sortKey)
        {
            // Decrease lifetime
            lifetime.ValueRW._lifetime -= deltaTime * lifetime.ValueRO.decreasingFactor;
            
            if (lifetime.ValueRO._lifetime > 0) return;
            
            if (lifetime.ValueRO.reproduced || lifetime.ValueRO.alwaysReproduce)
            {
                // Reset lifetime similar to Start() method
                lifetime.ValueRW._startingLifetime = Random.Range(5f, 15f);
                lifetime.ValueRW._lifetime = lifetime.ValueRO._startingLifetime;
                lifetime.ValueRW.reproduced = false;
                
                // Mark entity for respawn by adding RespawnTag
                // We use sortKey as the job index for thread safety
                ecb.AddComponent<RespawnTag>(sortKey, entity);
            }
            else
            {
                // In ECS we typically destroy entities rather than disable them
                ecb.DestroyEntity(sortKey, entity);
            }
        }
    }
}
