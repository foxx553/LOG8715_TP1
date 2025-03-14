using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;

public class VelocityAuthoring : MonoBehaviour
{
    // Add serialized field for setting initial velocity in Inspector
    [SerializeField] private Vector3 initialVelocity;
    
    class Baker : Baker<VelocityAuthoring>
    {
        public override void Bake(VelocityAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            // Convert Unity Vector3 to float3
            AddComponent(entity, new Velocity { velocity = authoring.initialVelocity });
        }
    }
}

public struct Velocity : IComponentData {
    public float3 velocity;
}