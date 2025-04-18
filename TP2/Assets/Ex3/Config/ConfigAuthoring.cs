using Unity.Entities;
using UnityEngine;

public class ConfigAuthoring : MonoBehaviour
{
    [Header("Entity Counts")]
    [SerializeField] public int plantCount = 3000;
    [SerializeField] public int preyCount = 3000;
    [SerializeField] public int predatorCount = 2500;

    [Header("World Settings")]
    [SerializeField] public int gridSize = 12000;

    [Header("Movement Settings")]
    [SerializeField] public float preySpeed = 1.0f;
    [SerializeField] public float predatorSpeed = 0.5f;

    [Header("Interaction Settings")]
    [SerializeField] public float touchingDistance = 0.5f;

    class Baker : Baker<ConfigAuthoring>
    {
        public override void Bake(ConfigAuthoring config)
        {
            var entity = GetEntity(TransformUsageFlags.None);
            AddComponent(entity, new ConfigComponent
            {
                plantCount = config.plantCount,
                preyCount = config.preyCount,
                predatorCount = config.predatorCount,
                gridSize = config.gridSize,
                preySpeed = config.preySpeed,
                predatorSpeed = config.predatorSpeed,
                touchingDistance = config.touchingDistance
            });
        }
    }
}

public struct ConfigComponent : IComponentData {
    public int plantCount;
    public int preyCount;
    public int predatorCount;
    public int gridSize;
    public float preySpeed;
    public float predatorSpeed;
    public float touchingDistance;
}