using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using static ECSComponents;

[BurstCompile]
public partial struct LifetimeSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        float deltaTime = SystemAPI.Time.DeltaTime;

        // Process predators
        foreach (var (lifetime, position) in SystemAPI.Query<RefRW<LifetimeData>, RefRO<LocalTransform>>().WithAll<PredatorTag>())
        {
            UpdateLifetimeBasedOnProximityToPrey(ref state, ref lifetime.ValueRW, position.ValueRO.Position, deltaTime);
        }

        // Process prey
        foreach (var (lifetime, position) in SystemAPI.Query<RefRW<LifetimeData>, RefRO<LocalTransform>>().WithAll<PreyTag>())
        {
            UpdateLifetimeBasedOnProximityToPlant(ref state, ref lifetime.ValueRW, position.ValueRO.Position, deltaTime);
            UpdateLifetimeBasedOnProximityToPredator(ref state, ref lifetime.ValueRW, position.ValueRO.Position, deltaTime);
        }

        // Process plants
        foreach (var (lifetime, position) in SystemAPI.Query<RefRW<LifetimeData>, RefRO<LocalTransform>>().WithAll<PlantTag>())
        {
            UpdateLifetimeBasedOnProximityToPrey(ref state, ref lifetime.ValueRW, position.ValueRO.Position, deltaTime);
        }
    }

    private void UpdateLifetimeBasedOnProximityToPrey(ref SystemState state, ref LifetimeData lifetime, float3 position, float deltaTime)
    {
        foreach (var (otherLifetime, otherPosition) in SystemAPI.Query<RefRO<LifetimeData>, RefRO<LocalTransform>>().WithAll<PreyTag>())
        {
            if (math.distance(position, otherPosition.ValueRO.Position) < Ex3Config.TouchingDistance)
            {
                lifetime.DecreasingFactor *= 2f; // Plants: Increase decreasing factor near prey
            }
        }
    }

    private void UpdateLifetimeBasedOnProximityToPlant(ref SystemState state, ref LifetimeData lifetime, float3 position, float deltaTime)
    {
        foreach (var (otherLifetime, otherPosition) in SystemAPI.Query<RefRO<LifetimeData>, RefRO<LocalTransform>>().WithAll<PlantTag>())
        {
            if (math.distance(position, otherPosition.ValueRO.Position) < Ex3Config.TouchingDistance)
            {
                lifetime.DecreasingFactor /= 2f; // Prey: Decrease decreasing factor near plants
            }
        }
    }

    private void UpdateLifetimeBasedOnProximityToPredator(ref SystemState state, ref LifetimeData lifetime, float3 position, float deltaTime)
    {
        foreach (var (otherLifetime, otherPosition) in SystemAPI.Query<RefRO<LifetimeData>, RefRO<LocalTransform>>().WithAll<PredatorTag>())
        {
            if (math.distance(position, otherPosition.ValueRO.Position) < Ex3Config.TouchingDistance)
            {
                lifetime.DecreasingFactor *= 2f; // Prey: Increase decreasing factor near predators
                lifetime.Reproduced = true;      // Predators: Mark as reproduced near prey
            }
        }
    }
}