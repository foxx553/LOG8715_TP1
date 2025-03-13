using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using static ECSComponents;

[BurstCompile]
public partial struct PredatorMovementSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        float deltaTime = SystemAPI.Time.DeltaTime;

        // Get all prey positions
        var preyPositions = SystemAPI.QueryBuilder().WithAll<PreyTag, LocalTransform>().Build().ToComponentDataArray<LocalTransform>(Allocator.TempJob);

        // Process predators
        foreach (var (velocity, position, movement) in SystemAPI.Query<RefRW<Velocity>, RefRO<LocalTransform>, RefRO<MovementData>>().WithAll<PredatorTag>())
        {
            float3 closestPreyPosition = FindClosestPosition(position.ValueRO.Position, preyPositions);
            velocity.ValueRW.Value = math.normalize(closestPreyPosition - position.ValueRO.Position) * movement.ValueRO.Speed;
        }

        preyPositions.Dispose();
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