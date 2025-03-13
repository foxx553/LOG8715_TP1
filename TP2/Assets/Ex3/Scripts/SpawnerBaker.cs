using Unity.Entities;
using UnityEngine;

public class SpawnerBaker : Baker<SpawnerAuthoring>
{
    public override void Bake(SpawnerAuthoring authoring)
    {
        // Create the entity for the spawner
        var entity = GetEntity(TransformUsageFlags.None);

        // Add the SpawnerConfig component to the entity
        AddComponent(entity, new SpawnerConfig
        {
            gridSize = authoring.config.gridSize,
            plantCount = authoring.config.plantCount,
            preyCount = authoring.config.preyCount,
            predatorCount = authoring.config.predatorCount,
            plantPrefab = GetEntity(authoring.plantPrefab, TransformUsageFlags.Dynamic),
            preyPrefab = GetEntity(authoring.preyPrefab, TransformUsageFlags.Dynamic),
            predatorPrefab = GetEntity(authoring.predatorPrefab, TransformUsageFlags.Dynamic)
        });
    }
}