using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using Random = Unity.Mathematics.Random;

[BurstCompile]
public partial struct SpawnSystem : ISystem
{
    private Random _random;
    private int _width;
    private int _height;
    private bool _hasInitialized;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<Spawner>();
        state.RequireForUpdate<ConfigComponent>();
        _random = Random.CreateFromIndex(1234); // Seed for reproducibility
        _hasInitialized = false;
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var spawner = SystemAPI.GetSingleton<Spawner>();
        var config = SystemAPI.GetSingleton<ConfigComponent>();
        var ecb = new EntityCommandBuffer(Allocator.Temp);

        float ratio = 16f / 9f; // Cant use camera in Burst Compile

        // Calculate grid dimensions once per frame
        var size = (float)config.gridSize;
        _height = (int)math.round(math.sqrt(size / ratio));
        _width = (int)math.round(size / _height);

        // Handle initial spawning of entities if not yet done
        if (!_hasInitialized)
        {
            Debug.Log("SpawnSystem: Starting initial spawning");
            InitialSpawn(ref state, ecb, spawner, config);
            _hasInitialized = true;
            Debug.Log("SpawnSystem: Initial spawning completed");
        }

        // Handle respawning of entities marked with RespawnTag
        HandleRespawning(ref state);

        // Play back the command buffer and dispose it
        ecb.Playback(state.EntityManager);
        ecb.Dispose();
    }

    private void InitialSpawn(ref SystemState state, EntityCommandBuffer ecb, Spawner spawner, ConfigComponent config)
    {
        Debug.Log($"Initializing simulation with: Plants={config.plantCount}, Prey={config.preyCount}, Predators={config.predatorCount}");

        // Spawn plants
        for (int i = 0; i < config.plantCount; i++)
        {
            Entity plantEntity = ecb.Instantiate(spawner.plantPrefab);
            PlaceRandomly(ecb, plantEntity);
        }

        // Spawn prey
        for (int i = 0; i < config.preyCount; i++)
        {
            Entity preyEntity = ecb.Instantiate(spawner.preyPrefab);
            PlaceRandomly(ecb, preyEntity);
        }

        // Spawn predators
        for (int i = 0; i < config.predatorCount; i++)
        {
            Entity predatorEntity = ecb.Instantiate(spawner.predatorPrefab);
            PlaceRandomly(ecb, predatorEntity);
        }
    }

    private void HandleRespawning(ref SystemState state)
    {
        var spawnPositions = new NativeArray<float3>(1000, Allocator.TempJob);

        for (int i = 0; i < spawnPositions.Length; i++)
        {
            spawnPositions[i] = GetRandomPosition();
        }

        var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
        var ecbParallel = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged).AsParallelWriter();

        // Schedule the respawn job
        state.Dependency = new RespawnJob
        {
            ecb = ecbParallel,
            randomPositions = spawnPositions
        }.ScheduleParallel(state.Dependency);

        // Register cleanup of native arrays
        state.Dependency = spawnPositions.Dispose(state.Dependency);
    }

    private void PlaceRandomly(EntityCommandBuffer ecb, Entity entity)
    {
        int halfWidth = _width / 2;
        int halfHeight = _height / 2;

        float3 position = new float3(
            _random.NextInt(-halfWidth, halfWidth),
            _random.NextInt(-halfHeight, halfHeight),
            0
        );

        ecb.SetComponent(entity, LocalTransform.FromPosition(position));
    }

    [BurstCompile]
    [WithAll(typeof(RespawnTag))]
    partial struct RespawnJob : IJobEntity
    {
        public EntityCommandBuffer.ParallelWriter ecb;
        [ReadOnly] public NativeArray<float3> randomPositions;
        
        void Execute(RefRW<LocalTransform> transform, Entity entity, [EntityIndexInQuery] int sortKey)
        {
            transform.ValueRW.Position = randomPositions[sortKey % randomPositions.Length];
            ecb.RemoveComponent<RespawnTag>(sortKey, entity);
        }
    }
    
    private float3 GetRandomPosition()
    {
        int halfWidth = _width / 2;
        int halfHeight = _height / 2;

        return new float3(
            _random.NextInt(-halfWidth, halfWidth),
            _random.NextInt(-halfHeight, halfHeight),
            0
        );
    }
}