using Unity.Entities;
using UnityEngine;

public class PlantBaker : Baker<PlantAuthoring>
{
    public override void Bake(PlantAuthoring authoring)
    {
        var entity = GetEntity(TransformUsageFlags.Dynamic);
        AddComponent(entity, new ScaleComponent { Value = 1.0f });
        AddComponent(entity, new LifetimeData
        {
            StartingLifetime = Random.Range(5f, 15f),
            CurrentLifetime = Random.Range(5f, 15f),
            DecreasingFactor = 1f,
            AlwaysReproduce = false,
            Reproduced = false
        });
        AddComponent<PlantTag>(entity);
    }
}

public class PreyBaker : Baker<PreyAuthoring>
{
    public override void Bake(PreyAuthoring authoring)
    {
        var entity = GetEntity(TransformUsageFlags.Dynamic);
        AddComponent(entity, new LifetimeData
        {
            StartingLifetime = Random.Range(5f, 15f),
            CurrentLifetime = Random.Range(5f, 15f),
            DecreasingFactor = 1f,
            AlwaysReproduce = false,
            Reproduced = false
        });
        AddComponent<PreyTag>(entity);
        AddComponent(entity, new MovementData { Speed = Ex3Config.PreySpeed });
        AddComponent<Velocity>(entity);
    }
}

public class PredatorBaker : Baker<PredatorAuthoring>
{
    public override void Bake(PredatorAuthoring authoring)
    {
        var entity = GetEntity(TransformUsageFlags.Dynamic);
        AddComponent(entity, new LifetimeData
        {
            StartingLifetime = Random.Range(5f, 15f),
            CurrentLifetime = Random.Range(5f, 15f),
            DecreasingFactor = 1f,
            AlwaysReproduce = false,
            Reproduced = false
        });
        AddComponent<PredatorTag>(entity);
        AddComponent(entity, new MovementData { Speed = Ex3Config.PredatorSpeed });
        AddComponent<Velocity>(entity);
    }
}