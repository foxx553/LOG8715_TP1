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

        uint id = 0;

        foreach (var shapeConfig in ecsController.Config.circleInstancesToSpawn)
        {
            componentDatabase.UpdatePositionComponent(id, shapeConfig.initialPosition);
            componentDatabase.UpdateVelocityComponent(id, shapeConfig.initialVelocity);
            componentDatabase.UpdateSizeComponent(id, shapeConfig.initialSize);
            if (componentDatabase.velocityComponent[id].Velocity == new Vector2(0f,0f)){
                componentDatabase.UpdateImmortalComponent(id, true);
            }
            ecsController.CreateShape(id, componentDatabase.sizeComponent[id].Size);
            ecsController.UpdateShapePosition(id, componentDatabase.positionComponent[id].Position);
            id++;            
        }
        _initialized = true;
    }
}