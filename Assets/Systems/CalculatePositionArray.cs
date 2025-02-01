using UnityEngine;

public class CalculatePositionArray : ISystem {
    public string Name => "CalculatePositionArray";
    private ComponentDatabaseArray _componentDatabase;
    public CalculatePositionArray(ComponentDatabaseArray componentDatabase) {
        _componentDatabase = componentDatabase;
    }
    public void UpdateSystem() {
        
        for (uint id = 0; id < _componentDatabase.entitiesCounter; id++){
            if (_componentDatabase.positionComponents[id] == null) continue;
            Vector2 position = _componentDatabase.positionComponents[id].Position;

            if (position.x > 0f && ((_componentDatabase.frameCounter % 4) != 0)) 
                    continue;

            if (_componentDatabase.velocityComponents[id] == null) continue;
            Vector2 velocity = _componentDatabase.velocityComponents[id].Velocity;

            if (velocity == null) continue;

            _componentDatabase.positionComponents[id].Position += velocity * (_componentDatabase.deltaTime);
        }
    }
}