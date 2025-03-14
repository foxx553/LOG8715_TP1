using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Random = Unity.Mathematics.Random;

[BurstCompile]
public partial struct SpawnSystem : ISystem
{
    private Random _random;
    private int _width;
    private int _height;
    
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<Spawner>();
        state.RequireForUpdate<ConfigComponent>();
        _random = Random.CreateFromIndex(1234); // Seed for reproducibility
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        // Calculate grid dimensions once per frame
        var config = SystemAPI.GetSingleton<ConfigComponent>();
        var size = (float)config.gridSize;
        var ratio = 16f/9f; // We can't use Camera.main in a job, so use a fixed ratio or provide from config
        
        _height = (int)math.round(math.sqrt(size / ratio));
        _width = (int)math.round(size / _height);
        
        // Get ECB and create a parallel writer
        var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
        var ecbParallel = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged).AsParallelWriter();
        
        // Create a NativeArray to store random positions - one per potential entity
        var spawnPositions = new NativeArray<float3>(1000, Allocator.TempJob);
        
        // Pre-calculate some random positions to use
        for (int i = 0; i < spawnPositions.Length; i++)
        {
            spawnPositions[i] = GetRandomPosition();
        }
        
        // Schedule the respawn job and update dependency chain
        // (removed .Complete() to allow async execution)
        state.Dependency = new RespawnJob
        {
            ecb = ecbParallel,
            randomPositions = spawnPositions
        }.ScheduleParallel(state.Dependency);
        
        // Register cleanup of native arrays in the dependency chain
        state.Dependency = spawnPositions.Dispose(state.Dependency);
    }
    
    [BurstCompile]
    partial struct RespawnJob : IJobEntity
    {
        public EntityCommandBuffer.ParallelWriter ecb;
        [ReadOnly] public NativeArray<float3> randomPositions;
        
        void Execute(RefRW<LocalTransform> transform, Entity entity, [EntityIndexInQuery] int sortKey)
        {
            // Get a position using the sortKey as an index (with wrapping)
            transform.ValueRW.Position = randomPositions[sortKey % randomPositions.Length];
            
            // Remove the respawn tag
            ecb.RemoveComponent<RespawnTag>(sortKey, entity);
        }
    }
    
    private float3 GetRandomPosition()
    {
        int halfWidth = _width / 2;
        int halfHeight = _height / 2;
        
        // Generate random position within grid bounds
        return new float3(
            _random.NextInt(-halfWidth, halfWidth),
            _random.NextInt(-halfHeight, halfHeight),
            0
        );
    }
}