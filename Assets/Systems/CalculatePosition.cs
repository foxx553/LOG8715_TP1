using UnityEngine;

public class CalculatePosition : ISystem {
    public string Name => "CalculatePosition";
    private ComponentDatabase _componentDatabase;
    public CalculatePosition(ComponentDatabase componentDatabase) {
        _componentDatabase = componentDatabase;
    }
    public void UpdateSystem() {
        foreach (var entry in _componentDatabase.positionComponent) {
            uint id = entry.Key;
            PositionComponent position = entry.Value;
            
            if (position.Position.x > 0f && ((_componentDatabase.frameCounter % 4) != 0)) continue;

            VelocityComponent velocity = _componentDatabase.velocityComponent[id];
            if (velocity.Velocity != null) {
                position.Position += velocity.Velocity * (4 * Time.deltaTime);
            }
        }
    }
}