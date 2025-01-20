using UnityEngine;

public class Initialization : ISystem{
    public string Name => "Initialization";
    private bool _initialized = false;
    
    [SerializeField] // Make it visible (Like a global variable)
    public ComponentDatabase componentDatabase;
    
    public Initialization() {
        if (componentDatabase == null) {
            componentDatabase = new ComponentDatabase();
            componentDatabase.startTime = Time.time;
        }
    }
    public void UpdateSystem(){
        if (_initialized)
            return;

        var ecsController = ECSController.Instance;

        uint id = 0;

        foreach (var shapeConfig in ecsController.Config.circleInstancesToSpawn)
        {
            ecsController.CreateShape(id, shapeConfig.initialSize);
            ecsController.UpdateShapePosition(id, shapeConfig.initialPosition);
            componentDatabase.UpdatePositionComponent(id, shapeConfig.initialPosition);
            componentDatabase.UpdateVelocityComponent(id, shapeConfig.initialVelocity);
            componentDatabase.UpdateSizeComponent(id, shapeConfig.initialSize);
            id++;            
        }
        _initialized = true;
    }
}

public class UpdatePosition : ISystem {
    public string Name => "UpdatePosition";

    private ComponentDatabase _componentDatabase;

    // Constructor
    public UpdatePosition(ComponentDatabase componentDatabase) {
        _componentDatabase = componentDatabase;
    }
    public void UpdateSystem() {
        var ecsController = ECSController.Instance; // temporaire ???
        foreach (var entry in _componentDatabase.positionComponent) {
            uint id = entry.Key;
            PositionComponent position = entry.Value;
            VelocityComponent velocity = _componentDatabase.velocityComponent[id];
            if (velocity != null) {
                position.Position += velocity.Velocity * Time.deltaTime;
            }
            _componentDatabase.UpdatePositionComponent(id, position.Position);
            ecsController.UpdateShapePosition(id, position.Position); // temporaire ???
            Debug.Log(position.Position);
        }
        Debug.Log("Position updated for all entities.");
    }
}


