using UnityEngine;

public class Initialization : ISystem{
    public string Name => "Initialization";
    private bool _initialized = false;
    
    [SerializeField] // Make it visible (Like a global variable)
    public ComponentDatabase componentDatabase;
    
    public Initialization() {
        if (componentDatabase == null) {
            componentDatabase = new ComponentDatabase();
        }
    }
    
    public void UpdateSystem(){
        if (_initialized)
            return;

        var ecsController = ECSController.Instance;

        foreach (var shapeConfig in ecsController.Config.circleInstancesToSpawn)
        {
            componentDatabase.UpdatePositionComponent(componentDatabase.entitiesCounter, shapeConfig.initialPosition);
            componentDatabase.UpdateVelocityComponent(componentDatabase.entitiesCounter, shapeConfig.initialVelocity);
            componentDatabase.UpdateSizeComponent(componentDatabase.entitiesCounter, shapeConfig.initialSize);
            if (componentDatabase.velocityComponent[componentDatabase.entitiesCounter].Velocity == new Vector2(0f,0f)){
                componentDatabase.UpdateImmortalComponent(componentDatabase.entitiesCounter, true);
            }
            componentDatabase.UpdateProtectionComponent(componentDatabase.entitiesCounter, 0);
            ecsController.CreateShape(componentDatabase.entitiesCounter, componentDatabase.sizeComponent[componentDatabase.entitiesCounter].Size);
            ecsController.UpdateShapePosition(componentDatabase.entitiesCounter, componentDatabase.positionComponent[componentDatabase.entitiesCounter].Position);
            componentDatabase.entitiesCounter++;            
        }
        _initialized = true;
    }
}