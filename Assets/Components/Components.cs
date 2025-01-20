using UnityEngine;

public class PositionComponent : IComponent{
    public Vector2 Position {get; set;}
}
public class VelocityComponent : IComponent{
    public Vector2 Velocity {get; set;}
}
public class SizeComponent : IComponent{
    public int Size {get; set;}
}
public class CollisionComponent : IComponent{
    public uint? Id {get; set;}
}
public class CooldownComponent : IComponent{
    public float DeltaTime {get; set;}
}