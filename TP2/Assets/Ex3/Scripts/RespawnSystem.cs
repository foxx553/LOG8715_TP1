using Unity.Entities;
using Unity.Burst;
using Unity.Mathematics;
using Unity.Transforms;
using Random = Unity.Mathematics.Random;

[BurstCompile]
public partial struct RespawnSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var random = SystemAPI.GetComponent<RandomComponent>(state.SystemHandle).Random;
        float deltaTime = SystemAPI.Time.DeltaTime;

        // Process entities with RespawnTag
        foreach (var (transform, entity) in SystemAPI.Query<RefRW<Translation>>().WithAll<RespawnTag>().WithEntityAccess())
        {
            // Respawn at a random position
            transform.ValueRW.Value = new float3(
                random.NextFloat(-10, 10),
                random.NextFloat(-10, 10),
                0
            );

            // Remove the RespawnTag
            state.EntityManager.RemoveComponent<RespawnTag>(entity);
        }

        // Update the random seed
        SystemAPI.SetComponent(state.SystemHandle, new RandomComponent { Random = random });
    }
}