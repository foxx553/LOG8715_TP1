using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class ComponentDatabaseArray
{
    public long startTime = 0;
    public float totalTime = 0f;
    public float deltaTime = 1f/30f;
    public uint frameCounter = 1;
    public uint entitiesCounter = 0;
    
    private const int InitialSize = 100;
    private int capacity = InitialSize;

    public List<uint> availableIds = new();
    public PositionComponent[] positionComponents = new PositionComponent[InitialSize];
    public VelocityComponent[] velocityComponents = new VelocityComponent[InitialSize];
    public SizeComponent[] sizeComponents = new SizeComponent[InitialSize];
    public IsStatic[] isStatics = new IsStatic[InitialSize];
    public IsImmortal[] isImmortals = new IsImmortal[InitialSize];
    public IsColliding[] isCollidings = new IsColliding[InitialSize];

    public IsExploded[] isExplodeds = new IsExploded[InitialSize];
    public IsProtectable[] isProtectables = new IsProtectable[InitialSize];
    public IsProtected[] isProtecteds = new IsProtected[InitialSize];
    public CooldownComponent[] cooldownComponents = new CooldownComponent[InitialSize];
    
    private void EnsureCapacity(uint id)
    {
        if (id >= capacity)
        {
            int newCapacity = capacity + 100;
            System.Array.Resize(ref positionComponents, newCapacity);
            System.Array.Resize(ref velocityComponents, newCapacity);
            System.Array.Resize(ref sizeComponents, newCapacity);
            System.Array.Resize(ref isStatics, newCapacity);
            System.Array.Resize(ref isImmortals, newCapacity);
            System.Array.Resize(ref isCollidings, newCapacity);
            System.Array.Resize(ref isProtectables, newCapacity);
            System.Array.Resize(ref isProtecteds, newCapacity);
            System.Array.Resize(ref isExplodeds, newCapacity);
            System.Array.Resize(ref cooldownComponents, newCapacity);
            capacity = newCapacity;
        }
    }

    public void UpdatePositionComponent(uint id, Vector2 position)
    {
        EnsureCapacity(id);
        positionComponents[id] = new PositionComponent { Position = position };
    }
    public void UpdateVelocityComponent(uint id, Vector2 velocity)
    {
        EnsureCapacity(id);
        velocityComponents[id] = new VelocityComponent { Velocity = velocity };
    }
    public void UpdateSizeComponent(uint id, int size)
    {
        EnsureCapacity(id);
        sizeComponents[id] = new SizeComponent { Size = size };
    }
    public void UpdateIsStatic(uint id)
    {
        EnsureCapacity(id);
        isStatics[id] = new IsStatic();
    }
    public void UpdateIsImmortal(uint id, bool isImmortal)
    {
        EnsureCapacity(id);
        isImmortals[id] = isImmortal ? new IsImmortal() : null;
    }
    public void UpdateIsColliding(uint id, bool isColliding)
    {
        EnsureCapacity(id);
        isCollidings[id] = isColliding ? new IsColliding() : null;
    }
    public void UpdateIsExploded(uint id, bool isExploded)
    {
        EnsureCapacity(id);
        isExplodeds[id] = isExploded ? new IsExploded() : null;
    }
    public void UpdateIsProtectable(uint id, int protectionCount)
    {
        EnsureCapacity(id);
        isProtectables[id] = new IsProtectable { ProtectionCount = protectionCount };
    }
    public void UpdateIsProtected(uint id, float deltaTime)
    {
        EnsureCapacity(id);
        isProtecteds[id] = new IsProtected { DeltaTime = deltaTime };
    }

    public void UpdateCooldown(uint id, float deltaTime)
    {
        EnsureCapacity(id);
        cooldownComponents[id] = new CooldownComponent { DeltaTime = deltaTime };
    }

    public void DestroyId(uint id)
    {
        if (id < capacity)
        {
            positionComponents[id] = null;
            velocityComponents[id] = null;
            sizeComponents[id] = null;
            isImmortals[id] = null;
            isProtectables[id] = null;
            isProtecteds[id] = null;
            cooldownComponents[id] = null;
        }
    }
}