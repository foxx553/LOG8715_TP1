using Unity.Entities;
using Unity.Burst;
using Unity.Mathematics;
using Unity.Transforms;

[BurstCompile]
public partial struct LifetimeSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        float deltaTime = SystemAPI.Time.DeltaTime;

        // Process plants
        foreach (var (lifetime, position) in SystemAPI.Query<RefRW<LifetimeData>, RefRO<Translation>>().WithAll<PlantTag>())
        {
            UpdateLifetimeBasedOnProximity<PreyTag>(ref lifetime.ValueRW, position.ValueRO.Value, deltaTime);
        }

        // Process prey
        foreach (var (lifetime, position) in SystemAPI.Query<RefRW<LifetimeData>, RefRO<Translation>>().WithAll<PreyTag>())
        {
            UpdateLifetimeBasedOnProximity<PlantTag>(ref lifetime.ValueRW, position.ValueRO.Value, deltaTime);
            UpdateLifetimeBasedOnProximity<PredatorTag>(ref lifetime.ValueRW, position.ValueRO.Value, deltaTime);
        }

        // Process predators
        foreach (var (lifetime, position) in SystemAPI.Query<RefRW<LifetimeData>, RefRO<Translation>>().WithAll<PredatorTag>())
        {
            UpdateLifetimeBasedOnProximity<PreyTag>(ref lifetime.ValueRW, position.ValueRO.Value, deltaTime);
        }
    }

    private void UpdateLifetimeBasedOnProximity<T>(ref LifetimeData lifetime, float3 position, float deltaTime) where T : unmanaged, IComponentData
    {
        foreach (var (otherLifetime, otherPosition) in SystemAPI.Query<RefRO<LifetimeData>, RefRO<Translation>>().WithAll<T>())
        {
            if (math.distance(position, otherPosition.ValueRO.Value) < Ex3Config.TouchingDistance)
            {
                if (typeof(T) == typeof(PreyTag))
                {
                    lifetime.DecreasingFactor *= 2f; // Plants: Increase decreasing factor near prey
                }
                else if (typeof(T) == typeof(PlantTag))
                {
                    lifetime.DecreasingFactor /= 2f; // Prey: Decrease decreasing factor near plants
                }
                else if (typeof(T) == typeof(PredatorTag))
                {
                    lifetime.DecreasingFactor *= 2f; // Prey: Increase decreasing factor near predators
                    lifetime.Reproduced = true;      // Predators: Mark as reproduced near prey
                }
            }
        }
    }
}