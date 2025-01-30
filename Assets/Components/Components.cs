using UnityEngine;
using System.Collections.Generic;

public class PositionComponent : IComponent
{
    public Vector2 Position { get; set; }
}
public class VelocityComponent : IComponent
{
    public Vector2 Velocity { get; set; }
}
public class SizeComponent : IComponent
{
    public int Size { get; set; }
}

public class IsStatic : IComponent { }
public class IsImmortal : IComponent { }

public class IsProtectable : IComponent
{
    public int ProtectionCount { get; set; }
}

public class IsProtected : IComponent
{
    public float DeltaTime { get; set; }
}

public class CooldownComponent : IComponent
{
    public float DeltaTime { get; set; }
}

public class IsColliding : IComponent { }