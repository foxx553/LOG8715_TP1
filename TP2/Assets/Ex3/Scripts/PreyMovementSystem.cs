using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[BurstCompile]
public partial struct PreyMovementSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        float deltaTime = SystemAPI.Time.DeltaTime;

        // Get all plant positions
        var plantPositions = SystemAPI.QueryBuilder().WithAll<PlantTag, Translation>().Build().ToComponentDataArray<Translation>(Allocator.TempJob);

        // Process prey
        foreach (var (velocity, position, movement) in SystemAPI.Query<RefRW<Velocity>, RefRO<Translation>, RefRO<MovementData>>().WithAll<PreyTag>())
        {
            float3 closestPlantPosition = FindClosestPosition(position.ValueRO.Value, plantPositions);
            velocity.ValueRW.Value = math.normalize(closestPlantPosition - position.ValueRO.Value) * movement.ValueRO.Speed;
        }

        plantPositions.Dispose();
    }

    private float3 FindClosestPosition(float3 sourcePosition, NativeArray<Translation> targets)
    {
        float closestDistance = float.MaxValue;
        float3 closestPosition = sourcePosition;

        foreach (var target in targets)
        {
            float distance = math.distance(sourcePosition, target.Value);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestPosition = target.Value;
            }
        }

        return closestPosition;
    }
}