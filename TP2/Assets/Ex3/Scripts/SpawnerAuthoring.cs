using UnityEngine;
using Unity.Entities;

// An authoring component is just a normal MonoBehavior that has a Baker<T> class.
public class SpawnerAuthoring : MonoBehaviour
{
    public GameObject predatorPrefab;
    public GameObject preyPrefab;
    public GameObject plantPrefab;

    // In baking, this Baker will run once for every SpawnerAuthoring instance in a subscene.
    // (Note that nesting an authoring component's Baker class inside the authoring MonoBehaviour class
    // is simply an optional matter of style.)
    class Baker : Baker<SpawnerAuthoring>
    {
        public override void Bake(SpawnerAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.None);
            AddComponent(entity, new Spawner
            {
                predatorPrefab = GetEntity(authoring.predatorPrefab, TransformUsageFlags.Dynamic),
                preyPrefab = GetEntity(authoring.preyPrefab, TransformUsageFlags.Dynamic),
                plantPrefab = GetEntity(authoring.plantPrefab, TransformUsageFlags.Dynamic),
            });
        }
    }
}

struct Spawner : IComponentData
{
    public Entity predatorPrefab;
    public Entity preyPrefab;
    public Entity plantPrefab;
}