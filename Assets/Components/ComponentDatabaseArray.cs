using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class ComponentDatabaseArray : System.ICloneable
{
    public long startTime = 0;
    public float deltaTime = 1f/30f;
    public uint frameCounter = 0;
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
            isExplodeds[id] = null;
            isCollidings[id] = null;
        }
    }

    public void ApplySave(ComponentDatabaseArray save) {
        this.startTime = save.startTime;
        this.deltaTime = save.deltaTime;
        this.frameCounter = save.frameCounter;
        this.entitiesCounter = save.entitiesCounter;
        this.capacity = save.capacity;
        this.availableIds = save.availableIds;

        this.positionComponents = save.positionComponents;
        this.velocityComponents = save.velocityComponents;
        this.sizeComponents = save.sizeComponents;
        this.isStatics = save.isStatics;
        this.isImmortals = save.isImmortals;
        this.isCollidings = save.isCollidings;
        this.isExplodeds = save.isExplodeds;
        this.isProtectables = save.isProtectables;
        this.isProtecteds = save.isProtecteds;
        this.cooldownComponents = save.cooldownComponents;
    }

    public object Clone()
    {
        ComponentDatabaseArray clonedInstance = new();
        clonedInstance.startTime = this.startTime;
        clonedInstance.deltaTime = this.deltaTime;
        clonedInstance.frameCounter = this.frameCounter;
        clonedInstance.entitiesCounter = this.entitiesCounter;
        clonedInstance.capacity = this.capacity;
        clonedInstance.availableIds = new List<uint>(this.availableIds);

        for (int i = 0; i < this.positionComponents.Length; i++) {
            if (this.positionComponents[i] != null)
                clonedInstance.positionComponents[i] = (PositionComponent) this.positionComponents[i].Clone();
        }

        for (int i = 0; i < this.velocityComponents.Length; i++) {
            if (this.velocityComponents[i] != null)
                clonedInstance.velocityComponents[i] = (VelocityComponent) this.velocityComponents[i].Clone();
        }

        for (int i = 0; i < this.sizeComponents.Length; i++) {
            if (this.sizeComponents[i] != null)
                clonedInstance.sizeComponents[i] = (SizeComponent) this.sizeComponents[i].Clone();
        }

        for (int i = 0; i < this.isStatics.Length; i++) {
            if (this.isStatics[i] != null)
                clonedInstance.isStatics[i] = (IsStatic) this.isStatics[i].Clone();
        }

        for (int i = 0; i < this.isImmortals.Length; i++) {
            if (this.isImmortals[i] != null)
                clonedInstance.isImmortals[i] = (IsImmortal) this.isImmortals[i].Clone();
        }

        for (int i = 0; i < this.isCollidings.Length; i++) {
            if (this.isCollidings[i] != null)
                clonedInstance.isCollidings[i] = (IsColliding) this.isCollidings[i].Clone();
        }

        for (int i = 0; i < this.isExplodeds.Length; i++) {
            if (this.isExplodeds[i] != null)
                clonedInstance.isExplodeds[i] = (IsExploded) this.isExplodeds[i].Clone();
        }

        for (int i = 0; i < this.isProtectables.Length; i++) {
            if (this.isProtectables[i] != null)
                clonedInstance.isProtectables[i] = (IsProtectable) this.isProtectables[i].Clone();
        }

        for (int i = 0; i < this.isProtecteds.Length; i++) {
            if (this.isProtecteds[i] != null)
                clonedInstance.isProtecteds[i] = (IsProtected) this.isProtecteds[i].Clone();
        }

        for (int i = 0; i < this.cooldownComponents.Length; i++) {
            if (this.cooldownComponents[i] != null)
                clonedInstance.cooldownComponents[i] = (CooldownComponent) this.cooldownComponents[i].Clone();
        }

        return clonedInstance;
    }
}