using Unity.Entities;
using Unity.Mathematics;

// Define all ECS components in a single class
public static class ECSComponents
{
    // === Tags ===
    // Tags are used to differentiate entities
    public struct PlantTag : IComponentData { }
    public struct PreyTag : IComponentData { }
    public struct PredatorTag : IComponentData { }

    // === Data Components ===
    // Lifetime component
    public struct Lifetime : IComponentData
    {
        public float Value; // Represents the remaining lifetime
    }

    // Velocity component
    public struct Velocity : IComponentData
    {
        public float3 Value; // Represents movement direction and speed
    }

    // Position component (optional, since Unity.Transforms.LocalTransform already exists)
    public struct Position : IComponentData
    {
        public float3 Value; // Represents the entity's position
    }

    // Health component
    public struct Health : IComponentData
    {
        public float Value; // Represents the entity's health
    }

    // === Configuration Components ===
    // Spawner configuration component
    public struct SpawnerConfig : IComponentData
    {
        public int gridSize;
        public int plantCount;
        public int preyCount;
        public int predatorCount;
        public Entity plantPrefab;
        public Entity preyPrefab;
        public Entity predatorPrefab;
    }

    // === Other Components ===
    // Example: A component to track if an entity is active
    public struct IsActive : IComponentData
    {
        public bool Value;
    }

    
    public struct LifetimeData : IComponentData
    {
        public float StartingLifetime; // Initial lifetime value
        public float CurrentLifetime;  // Current lifetime value
        public float DecreasingFactor; // Rate at which lifetime decreases
        public bool AlwaysReproduce;   // Whether the entity should always reproduce
        public bool Reproduced;        // Whether the entity has reproduced
    }

    
    // Tag to mark entities that need to reproduce
    public struct ReproduceTag : IComponentData { }

    // Tag to mark entities that need to respawn
    public struct RespawnTag : IComponentData { }

    public struct ProximityData : IComponentData
    {
        public float TouchingDistance; // Distance at which entities are considered "touching"
    }

    // Component to hold the random seed
    public struct RandomComponent : IComponentData
    {
        public Random Random;
    }

    public struct MovementData : IComponentData
    {
        public float Speed; // Movement speed
    }

    public struct ScaleComponent : IComponentData
    {
        public float Value; // Current scale value
    }
}