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
public class ImmortalComponent : IComponent
{
    public bool IsImmortal { get; set; }
}
public class CooldownComponent : IComponent
{
    public float DeltaTime { get; set; }
}

public class IsColliding : IComponent { }