using UnityEngine;

public class InitializationArray : ISystem{
    public string Name => "InitializationArray";
    private bool _initialized = false;
    
    [SerializeField] // Make it visible (Like a global variable)
    public ComponentDatabaseArray componentDatabase;
    
    public InitializationArray() {
        if (componentDatabase == null) {
            componentDatabase = new ComponentDatabaseArray();
        }
    }
    
    public void UpdateSystem(){
        if (_initialized)
            return;
            
        var ecsController = ECSController.Instance;
        foreach (var shapeConfig in ecsController.Config.circleInstancesToSpawn)
        {
            uint id = componentDatabase.entitiesCounter;

            componentDatabase.UpdatePositionComponent(id, shapeConfig.initialPosition);
            componentDatabase.UpdateVelocityComponent(id, shapeConfig.initialVelocity);
            componentDatabase.UpdateSizeComponent(id, shapeConfig.initialSize);

            if (componentDatabase.velocityComponents[id].Velocity == Vector2.zero){
                componentDatabase.UpdateIsStatic(id);
                componentDatabase.UpdateIsImmortal(id, true);
            }
            if (componentDatabase.sizeComponents[id].Size == ecsController.Config.protectionSize)
            {
                componentDatabase.UpdateIsProtectable(id, 0);
            }
            ecsController.CreateShape(id, componentDatabase.sizeComponents[id].Size);
            componentDatabase.entitiesCounter++;
        }
        _initialized = true;
    }
}