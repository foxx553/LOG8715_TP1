using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using static ECSComponents;

[BurstCompile]
public partial struct PreyMovementSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        float deltaTime = SystemAPI.Time.DeltaTime;

        // Get all plant positions
        var plantPositions = SystemAPI.QueryBuilder().WithAll<PlantTag, LocalTransform>().Build().ToComponentDataArray<LocalTransform>(Allocator.TempJob);

        // Process prey
        foreach (var (velocity, position, movement) in SystemAPI.Query<RefRW<Velocity>, RefRO<LocalTransform>, RefRO<MovementData>>().WithAll<PreyTag>())
        {
            float3 closestPlantPosition = FindClosestPosition(position.ValueRO.Position, plantPositions);
            velocity.ValueRW.Value = math.normalize(closestPlantPosition - position.ValueRO.Position) * movement.ValueRO.Speed;
        }

        plantPositions.Dispose();
    }

    private float3 FindClosestPosition(float3 sourcePosition, NativeArray<LocalTransform> targets)
    {
        float closestDistance = float.MaxValue;
        float3 closestPosition = sourcePosition;

        foreach (var target in targets)
        {
            float distance = math.distance(sourcePosition, target.Position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestPosition = target.Position;
            }
        }

        return closestPosition;
    }
}