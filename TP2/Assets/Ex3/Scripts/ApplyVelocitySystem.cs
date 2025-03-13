using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[BurstCompile]
public partial struct ApplyVelocitySystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        float deltaTime = SystemAPI.Time.DeltaTime;

        // Process all entities with Velocity and Translation
        foreach (var (velocity, position) in SystemAPI.Query<RefRO<Velocity>, RefRW<Translation>>())
        {
            position.ValueRW.Value += velocity.ValueRO.Value * deltaTime;
        }
    }
}