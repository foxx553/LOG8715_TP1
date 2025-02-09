using UnityEngine;
using System;

public class InitializationArray : ISystem{
    public string Name => "InitializationArray";
    private bool _initialized = false;
    private ComponentDatabaseArray _componentDatabase;
    
    public InitializationArray(ComponentDatabaseArray componentDatabase) {
        _componentDatabase = componentDatabase;
    }
    
    public void UpdateSystem(){
        if (_initialized)
            return;
        _componentDatabase.startTime = ((DateTime.UtcNow.Hour * 60 
                                        + DateTime.UtcNow.Minute) * 60 
                                        + DateTime.UtcNow.Second) * 1000  
                                        + DateTime.UtcNow.Millisecond;
        var ecsController = ECSController.Instance;
        foreach (var shapeConfig in ecsController.Config.circleInstancesToSpawn)
        {
            uint id = _componentDatabase.entitiesCounter;

            _componentDatabase.UpdatePositionComponent(id, shapeConfig.initialPosition);
            _componentDatabase.UpdateVelocityComponent(id, shapeConfig.initialVelocity);
            _componentDatabase.UpdateSizeComponent(id, shapeConfig.initialSize);

            if (_componentDatabase.velocityComponents[id].Velocity == Vector2.zero){
                _componentDatabase.UpdateIsStatic(id);
                _componentDatabase.UpdateIsImmortal(id, true);
            }
            ecsController.CreateShape(id, _componentDatabase.sizeComponents[id].Size);
            ecsController.UpdateShapePosition(id, shapeConfig.initialPosition);
            _componentDatabase.entitiesCounter++;
        }
        _initialized = true;
    }
}