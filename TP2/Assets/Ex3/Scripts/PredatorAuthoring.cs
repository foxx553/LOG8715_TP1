using UnityEngine;
using Unity.Entities;

public class PredatorAuthoring : MonoBehaviour
{
    class Baker : Baker<PredatorAuthoring>
    {
        public override void Bake(PredatorAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new Predator());
        }
    }
}

public struct Predator : IComponentData { }