using UnityEngine;
using System.Collections.Generic;

public class PositionComponent : IComponent, System.ICloneable
{
    public Vector2 Position { get; set; }

    public object Clone()
    {
        return new PositionComponent { Position = this.Position };
    }
}
public class VelocityComponent : IComponent, System.ICloneable
{
    public Vector2 Velocity { get; set; }

    public object Clone()
    {
        return new VelocityComponent { Velocity = this.Velocity };
    }
}
public class SizeComponent : IComponent, System.ICloneable
{
    public int Size { get; set; }

    public object Clone()
    {
        return new SizeComponent { Size = this.Size };
    }
}

public class IsStatic : IComponent, System.ICloneable
{
    public object Clone()
    {
        return new IsStatic();
    }
}
public class IsImmortal : IComponent, System.ICloneable { 

    public object Clone()
    {
        return new IsImmortal();
    }

}

public class IsProtectable : IComponent, System.ICloneable
{
    public int ProtectionCount { get; set; }

    public object Clone()
    {
        return new IsProtectable { ProtectionCount = this.ProtectionCount } ;
    }
}

public class IsProtected : IComponent, System.ICloneable
{
    public float DeltaTime { get; set; }

    public object Clone()
    {
        return new IsProtected { DeltaTime = this.DeltaTime };
    }
}

public class CooldownComponent : IComponent, System.ICloneable
{
    public float DeltaTime { get; set; }

    public object Clone()
    {
        return new CooldownComponent { DeltaTime = this.DeltaTime };
    }
}

public class IsColliding : IComponent, System.ICloneable
{
    public object Clone()
    {
        return new IsColliding();
    }
}

public class IsExploded : IComponent, System.ICloneable {

    public object Clone()
    {
        return new IsExploded();
    }

}