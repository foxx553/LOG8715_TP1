using Unity.Entities;
using UnityEngine;

public class ConfigAuthoring : MonoBehavior {
    class Baker : Baker<ConfigAuthoring> {
        public override void Bake(ConfigAuthoring config) {
            var entity = GetEntity(TransformUsageFlags.None);
            AddComponent(entity, new ConfigComponent {
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