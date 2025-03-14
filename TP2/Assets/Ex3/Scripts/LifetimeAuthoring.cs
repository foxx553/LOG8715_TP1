using UnityEngine;
using Unity.Entities;

public class LifetimeAuthoring : MonoBehaviour
{
    private const float StartingLifetimeLowerBound = 5;
    private const float StartingLifetimeUpperBound = 15;
    
    // Add serialized field for Inspector
    [SerializeField] public bool alwaysReproduce;
    [SerializeField] public float decreasingFactor = 1;

    class Baker : Baker<LifetimeAuthoring>
    {
        public override void Bake(LifetimeAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            
            // Calculate random starting lifetime
            float startingLifetime = Random.Range(StartingLifetimeLowerBound, StartingLifetimeUpperBound);
            
            AddComponent(entity, new Lifetime
            {
                _startingLifetime = startingLifetime,
                _lifetime = startingLifetime, // Now correctly using local variable
                decreasingFactor = authoring.decreasingFactor, // Using Inspector value
                alwaysReproduce = authoring.alwaysReproduce, // Using Inspector value
                reproduced = false
            });
        }
    }
}

public struct Lifetime : IComponentData {
    public float _startingLifetime;
    public float _lifetime;
    public float decreasingFactor;
    public bool alwaysReproduce;
    public bool reproduced;
}