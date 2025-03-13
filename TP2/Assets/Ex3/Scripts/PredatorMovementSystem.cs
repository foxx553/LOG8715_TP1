using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[BurstCompile]
public partial struct PredatorMovementSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        float deltaTime = SystemAPI.Time.DeltaTime;

        // Get all prey positions
        var preyPositions = SystemAPI.QueryBuilder().WithAll<PreyTag, Translation>().Build().ToComponentDataArray<Translation>(Allocator.TempJob);

        // Process predators
        foreach (var (velocity, position, movement) in SystemAPI.Query<RefRW<Velocity>, RefRO<Translation>, RefRO<MovementData>>().WithAll<PredatorTag>())
        {
            float3 closestPreyPosition = FindClosestPosition(position.ValueRO.Value, preyPositions);
            velocity.ValueRW.Value = math.normalize(closestPreyPosition - position.ValueRO.Value) * movement.ValueRO.Speed;
        }

        preyPositions.Dispose();
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