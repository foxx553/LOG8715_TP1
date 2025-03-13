using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Rendering;
using UnityEngine;

public partial class SpawnerSystem : SystemBase
{
    protected override void OnUpdate()
    {
        // Get the SpawnerConfig component
        var config = GetSingleton<SpawnerConfig>();

        // Spawn plants, prey, and predators
        SpawnEntities(config.plantCount, config.plantPrefab);
        SpawnEntities(config.preyCount, config.preyPrefab);
        SpawnEntities(config.predatorCount, config.predatorPrefab);

        // Disable the system after spawning to prevent repeated spawning
        Enabled = false;
    }

    private void SpawnEntities(int count, Entity prefab)
    {
        var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

        for (int i = 0; i < count; i++)
        {
            var instance = entityManager.Instantiate(prefab);
            var position = new float3(Random.Range(-10, 10), Random.Range(-10, 10), 0); // Random position
            entityManager.SetComponentData(instance, new Translation { Value = position });
        }
    }
}