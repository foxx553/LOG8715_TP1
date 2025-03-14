using UnityEngine;
using Unity.Entities;

public class PreyAuthoring : MonoBehaviour 
{
    class Baker : Baker<PreyAuthoring> {
        public override void Bake(PreyAuthoring authoring) {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new Prey());
        }
    }
}

public struct Prey : IComponentData {}