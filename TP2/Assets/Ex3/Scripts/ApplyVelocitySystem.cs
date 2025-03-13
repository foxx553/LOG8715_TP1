using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using static ECSComponents;

[BurstCompile]
public partial struct ApplyVelocitySystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        float deltaTime = SystemAPI.Time.DeltaTime;

        // Process all entities with Velocity and LocalTransform
        foreach (var (velocity, position) in SystemAPI.Query<RefRO<Velocity>, RefRW<LocalTransform>>())
        {
            position.ValueRW.Position += velocity.ValueRO.Value * deltaTime;
        }
    }
}