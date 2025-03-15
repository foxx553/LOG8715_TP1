using Unity.Burst;
using Unity.Entities;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;

[BurstCompile]
public partial struct LifetimeSystem : ISystem
{
    private uint _randomSeed;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<Lifetime>();
        // Initialize with a fixed seed that doesn't require DateTime.Now
        // We'll make it dynamic in OnUpdate anyway
        _randomSeed = 12345;
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        // We use the singleton ECB approach here because:
        // 1. We need a ParallelWriter for thread safety in jobs
        // 2. ECB playback is scheduled automatically after our system runs
        var ecbSingleton = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>();
        var ecbParallel = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged).AsParallelWriter();

        // Create and update our random seed using uint2 for explicit typing
        // Use system time as part of the seed to vary results between frames
        uint frameNumber = (uint)SystemAPI.Time.ElapsedTime * 1000;
        _randomSeed = math.hash(new uint2(_randomSeed, frameNumber));

        // Schedule the job and update dependency chain
        state.Dependency = new LifetimeJob
        {
            deltaTime = SystemAPI.Time.DeltaTime,
            ecb = ecbParallel,
            randomSeed = _randomSeed
        }.ScheduleParallel(state.Dependency);
    }

    // IJobEntity automatically iterates through all entities with the specified components
    [BurstCompile]
    partial struct LifetimeJob : IJobEntity
    {
        public float deltaTime;
        public EntityCommandBuffer.ParallelWriter ecb;
        public uint randomSeed;

        // This method will be called once per matching entity
        void Execute(RefRW<Lifetime> lifetime, Entity entity, [EntityIndexInQuery] int sortKey)
        {
            // Decrease lifetime
            lifetime.ValueRW._lifetime -= deltaTime * lifetime.ValueRO.decreasingFactor;
            
            if (lifetime.ValueRO._lifetime > 0) return;
            
            if (lifetime.ValueRO.reproduced || lifetime.ValueRO.alwaysReproduce)
            {
                // Create a random number generator that's unique per entity
                var random = Unity.Mathematics.Random.CreateFromIndex(randomSeed + (uint)sortKey);

                // Reset lifetime using Unity.Mathematics random instead of UnityEngine.Random
                lifetime.ValueRW._startingLifetime = random.NextFloat(5f, 15f);
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
