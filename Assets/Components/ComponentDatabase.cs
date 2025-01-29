using System.Collections.Generic; // generic collection of unity
using UnityEngine; // to have access to Vector2 type

[System.Serializable]
public class ComponentDatabase
{

    public float startTime = 0;
    public uint entitiesCounter = 0;
    public readonly Dictionary<uint, PositionComponent> positionComponent = new();
    public readonly Dictionary<uint, VelocityComponent> velocityComponent = new();
    public readonly Dictionary<uint, SizeComponent> sizeComponent = new();
    public readonly Dictionary<uint, IsStatic> isStatic = new();
    public readonly Dictionary<uint, ImmortalComponent> immortalComponent = new();
    public readonly Dictionary<uint, IsColliding> isColliding = new();
    public readonly Dictionary<uint, IsProtectable> isProtectable = new();
    public readonly Dictionary<uint, IsProtected> isProtected = new();

    private void UpdateComponent<T>(Dictionary<uint, T> componentDict, uint id, T newComponent)
    {
        if (componentDict.ContainsKey(id))
        {
            componentDict[id] = newComponent;
        }
        else
        {
            componentDict.Add(id, newComponent);
        }
    }
    public void UpdatePositionComponent(uint id, Vector2 position)
    {
        UpdateComponent(positionComponent, id, new PositionComponent { Position = position });
    }
    public void UpdateVelocityComponent(uint id, Vector2 velocity)
    {
        UpdateComponent(velocityComponent, id, new VelocityComponent { Velocity = velocity });
    }
    public void UpdateSizeComponent(uint id, int size)
    {
        UpdateComponent(sizeComponent, id, new SizeComponent { Size = size });
    }
    public void UpdateIsStatic(uint id, int size)
    {
        UpdateComponent(isStaticComponent, id, new isStatic());
    }
    public void UpdateImmortalComponent(uint id, bool isImmortal)
    {
        UpdateComponent(immortalComponent, id, new ImmortalComponent { IsImmortal = isImmortal });
    }

    public void UpdateIsCollidiing(uint id, bool isColliding)
    {
        if (isColliding)
            UpdateComponent(isCollidingComponent, id, new IsColliding());
        else isCollidingComponent.Remove(id);

    }

    public void UpdateIsProtectable(uint id, int protectionCount)
    {
        UpdateComponent(isProtectable, id, new IsProtectable { ProtectionCount = protectionCount });
    }

    public void UpdateIsProtected(uint id, int countdown)
    {
        UpdateComponent(isProtectable, id, new IsProtected { CountDown = countdown });
    }

    public void DestroyId(uint id){
        positionComponent.Remove(id);
        velocityComponent.Remove(id);
        sizeComponent.Remove(id);
        immortalComponent.Remove(id);
        isColliding.Remove(id);
        isProtectable.Remove(id);
        isProtected.Remove(id);
    }
}