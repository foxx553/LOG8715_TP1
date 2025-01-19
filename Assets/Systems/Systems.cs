using UnityEngine;

public class Initialization : ISystem{
    public string Name => "Initialization";
    private bool _initialized = false;
    
    public void UpdateSystem(){
        if (_initialized)
            return;

        var ecsController = ECSController.Instance;
        public ComponentDatabase componentDatabase;

        uint id = 0;

        foreach (var shapeConfig in ecsController.Config.circleInstancesToSpawn)
        {
            ecsController.CreateShape(id, shapeConfig.initialSize);
            ecsController.UpdateShapePosition(id, shapeConfig.initialPosition);
            id++;
        }

        

        _initialized = true;
    }
}