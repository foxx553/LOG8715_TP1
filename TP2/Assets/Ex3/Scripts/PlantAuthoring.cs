using UnityEngine;
using Unity.Entities;

public class PlantAuthoring : MonoBehaviour
{
    class Baker : Baker<PlantAuthoring>
    {
        public override void Bake(PlantAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new Plant());
        }
    }
}

public struct Plant : IComponentData { }