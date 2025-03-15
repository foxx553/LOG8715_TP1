using Unity.Entities;
using UnityEngine;

// This component holds references to prefabs for spawning
public class SpawnerAuthoring : MonoBehaviour
{
    public GameObject plantPrefab;
    public GameObject preyPrefab;
    public GameObject predatorPrefab;

    class Baker : Baker<SpawnerAuthoring>
    {
        public override void Bake(SpawnerAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.None);
            AddComponent(entity, new Spawner
            {
                plantPrefab = GetEntity(authoring.plantPrefab, TransformUsageFlags.Dynamic),
                preyPrefab = GetEntity(authoring.preyPrefab, TransformUsageFlags.Dynamic),
                predatorPrefab = GetEntity(authoring.predatorPrefab, TransformUsageFlags.Dynamic)
            });
        }
    }
}

// The ECS component that holds entity prefab references
public struct Spawner : IComponentData
{
    public Entity plantPrefab;
    public Entity preyPrefab;
    public Entity predatorPrefab;
}