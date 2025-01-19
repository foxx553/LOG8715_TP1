using UnityEngine;

public class PositionComponent : IComponent{
    public Vector2 Position {get; set;}
}
public class VelocityComponent : IComponent{
    public Vector2 Velocity {get; set;}
}

public class SizeComponent : IComponent{
    public uint Size {get; set;}
}